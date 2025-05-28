##########################################################################
# REQUIRED VARIABLES
# var.project_name      – prefix for names/tags
# var.vpc_id            – VPC where the instance will live
# var.vpc_cidr          – e.g. "10.0.0.0/16"
# var.subnet_id         – public (or private with NAT) subnet ID
# var.instance_type     – e.g. "t3.small"
# var.root_volume_size  – e.g. 30
# var.associate_public_ip (bool) – default true
##########################################################################

# Latest Amazon Linux 2 ECS-optimised AMI
data "aws_ami" "ecs_optimized" {
  most_recent = true
  owners      = ["amazon"]

  filter {
    name   = "name"
    values = ["amzn2-ami-ecs-hvm-*-x86_64-ebs"]
  }

  filter {
    name   = "virtualization-type"
    values = ["hvm"]
  }
}

########################
# SECURITY GROUP
########################
resource "aws_security_group" "ec2_sg" {
  name_prefix = "${var.project_name}-ec2-sg"
  vpc_id      = var.vpc_id
  description = "SG for ECS container instances"

  ingress {
    description = "SSH"
    from_port   = 22
    to_port     = 22
    protocol    = "tcp"
    cidr_blocks = ["0.0.0.0/0"]  # tighten in production
  }

  # dynamic ports for ALB/NLB + tasks running in bridge mode
  ingress {
    description     = "ALB to ECS dynamic ports"
    from_port       = 32768
    to_port         = 65535
    protocol        = "tcp"
    security_groups = [var.alb_security_group_id]
  }

  ingress {
    description = "VPC internal traffic on dynamic ports"
    from_port   = 32768
    to_port     = 65535
    protocol    = "tcp"
    cidr_blocks = [var.vpc_cidr]
  }

  egress {
    description = "All outbound"
    from_port   = 0
    to_port     = 0
    protocol    = "-1"
    cidr_blocks = ["0.0.0.0/0"]
  }

  tags = { Name = "${var.project_name}-ec2-sg" }
}

##################
# IAM FOR EC2
##################
resource "aws_iam_role" "ec2_role" {
  name = "${var.project_name}-ec2-role"

  assume_role_policy = jsonencode({
    Version = "2012-10-17"
    Statement = [{
      Action = "sts:AssumeRole"
      Effect = "Allow"
      Principal = { Service = "ec2.amazonaws.com" }
    }]
  })
}

resource "aws_iam_role_policy_attachment" "ecs_instance_policy" {
  role       = aws_iam_role.ec2_role.name
  policy_arn = "arn:aws:iam::aws:policy/service-role/AmazonEC2ContainerServiceforEC2Role"
}

resource "aws_iam_role_policy_attachment" "ssm_instance_policy" {
  role       = aws_iam_role.ec2_role.name
  policy_arn = "arn:aws:iam::aws:policy/AmazonSSMManagedInstanceCore"
}

resource "aws_iam_instance_profile" "ec2_profile" {
  name = "${var.project_name}-ec2-profile"
  role = aws_iam_role.ec2_role.name
}

#################################
# OPTIONAL KEY-PAIR
#################################
resource "tls_private_key" "ec2_key" {
  algorithm = "RSA"
  rsa_bits  = 4096
}

resource "aws_key_pair" "ec2_key" {
  key_name   = "${var.project_name}-ec2-key"
  public_key = tls_private_key.ec2_key.public_key_openssh
}

#################################
# ECS CLUSTER
#################################
resource "aws_ecs_cluster" "main" {
  name = "${var.project_name}-cluster"

  setting {
    name  = "containerInsights"
    value = "enabled"
  }

  tags = { Name = "${var.project_name}-cluster" }
}

#################################
# USER-DATA – join the cluster
#################################
locals {
  ecs_user_data = base64encode(<<-EOF
    #!/bin/bash
    echo 'ECS_CLUSTER=${aws_ecs_cluster.main.name}' >> /etc/ecs/ecs.config
  EOF
  )
}

#################################
# EC2 CONTAINER INSTANCE
#################################
resource "aws_instance" "ecs_instance" {
  ami                    = data.aws_ami.ecs_optimized.id
  instance_type          = var.instance_type
  subnet_id              = var.subnet_id
  vpc_security_group_ids = [aws_security_group.ec2_sg.id]
  key_name               = aws_key_pair.ec2_key.key_name
  iam_instance_profile   = aws_iam_instance_profile.ec2_profile.name
  user_data_base64       = local.ecs_user_data

  monitoring   = true
  ebs_optimized = var.instance_type != "t2.micro"

  root_block_device {
    volume_size           = var.root_volume_size
    volume_type           = "gp3"
    delete_on_termination = true
    encrypted             = true
    tags                  = { Name = "${var.project_name}-ec2-root" }
  }

  tags = {
    Name              = "${var.project_name}-ecs-instance"
    AmazonECSManaged  = "true"
    Cluster           = aws_ecs_cluster.main.name
  }

  lifecycle { create_before_destroy = true }
}

#############################
# OPTIONAL ELASTIC IP
#############################
resource "aws_eip" "ec2_eip" {
  count    = var.associate_public_ip ? 1 : 0
  instance = aws_instance.ecs_instance.id
  domain   = "vpc"

  tags = { Name = "${var.project_name}-ec2-eip" }
}

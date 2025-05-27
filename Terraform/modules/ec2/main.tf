# Get the latest ECS-optimized AMI
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

# Get availability zones
data "aws_availability_zones" "available" {
  state = "available"
}

# Security Group for EC2 instance
resource "aws_security_group" "ec2_sg" {
  name_prefix = "${var.project_name}-ec2-sg"
  vpc_id      = var.vpc_id
  description = "Security group for ECS EC2 instance"

  # SSH access
  ingress {
    description = "SSH"
    from_port   = 22
    to_port     = 22
    protocol    = "tcp"
    cidr_blocks = ["0.0.0.0/0"]  # Consider restricting this to your IP
  }

  # HTTP access
  ingress {
    description = "HTTP"
    from_port   = 80
    to_port     = 80
    protocol    = "tcp"
    cidr_blocks = ["0.0.0.0/0"]
  }

  # HTTPS access
  ingress {
    description = "HTTPS"
    from_port   = 443
    to_port     = 443
    protocol    = "tcp"
    cidr_blocks = ["0.0.0.0/0"]
  }

  # ECS agent port
  ingress {
    description = "ECS Agent"
    from_port   = 51678
    to_port     = 51678
    protocol    = "tcp"
    cidr_blocks = [var.vpc_cidr]
  }

  # Dynamic port range for ECS tasks
  ingress {
    description = "ECS Dynamic Ports"
    from_port   = 32768
    to_port     = 65535
    protocol    = "tcp"
    cidr_blocks = [var.vpc_cidr]
  }

  # All outbound traffic
  egress {
    description = "All outbound traffic"
    from_port   = 0
    to_port     = 0
    protocol    = "-1"
    cidr_blocks = ["0.0.0.0/0"]
  }

  tags = {
    Name = "${var.project_name}-ec2-sg"
  }
}

# IAM role for EC2 instance
resource "aws_iam_role" "ec2_role" {
  name = "${var.project_name}-ec2-role"

  assume_role_policy = jsonencode({
    Version = "2012-10-17"
    Statement = [
      {
        Action = "sts:AssumeRole"
        Effect = "Allow"
        Principal = {
          Service = "ec2.amazonaws.com"
        }
      }
    ]
  })

  tags = {
    Name = "${var.project_name}-ec2-role"
  }
}

# Attach ECS instance role policy
resource "aws_iam_role_policy_attachment" "ec2_role_policy" {
  role       = aws_iam_role.ec2_role.name
  policy_arn = "arn:aws:iam::aws:policy/service-role/AmazonEC2ContainerServiceforEC2Role"
}

# Attach SSM managed instance core policy for Systems Manager access
resource "aws_iam_role_policy_attachment" "ec2_ssm_policy" {
  role       = aws_iam_role.ec2_role.name
  policy_arn = "arn:aws:iam::aws:policy/AmazonSSMManagedInstanceCore"
}

# IAM instance profile
resource "aws_iam_instance_profile" "ec2_profile" {
  name = "${var.project_name}-ec2-profile"
  role = aws_iam_role.ec2_role.name

  tags = {
    Name = "${var.project_name}-ec2-profile"
  }
}

# Key pair for EC2 access - uses local public_key.pem file
resource "aws_key_pair" "ec2_key" {
  key_name   = "${var.project_name}-ec2-key"
  public_key = file("${path.module}/public_key.pem")

  tags = {
    Name = "${var.project_name}-ec2-key"
  }
}

# ECS Cluster
resource "aws_ecs_cluster" "main" {
  name = "${var.project_name}-cluster"

  setting {
    name  = "containerInsights"
    value = "enabled"
  }

  tags = {
    Name = "${var.project_name}-cluster"
  }
}

# User data script for ECS agent configuration
locals {
  user_data = base64encode(templatefile("${path.module}/user_data.sh", {
    cluster_name = aws_ecs_cluster.main.name
  }))
}

# EC2 Instance
resource "aws_instance" "ecs_instance" {
  ami                     = data.aws_ami.ecs_optimized.id
  instance_type          = var.instance_type
  key_name               = aws_key_pair.ec2_key.key_name
  vpc_security_group_ids = [aws_security_group.ec2_sg.id]
  subnet_id              = var.subnet_id
  iam_instance_profile   = aws_iam_instance_profile.ec2_profile.name
  
  user_data = local.user_data

  # Enable detailed monitoring (free tier eligible)
  monitoring = false

  # EBS optimization (not available for t2.micro)
  ebs_optimized = var.instance_type != "t2.micro" ? true : false

  root_block_device {
    volume_type           = "gp3"
    volume_size           = var.root_volume_size
    delete_on_termination = true
    encrypted             = true

    tags = {
      Name = "${var.project_name}-ec2-root-volume"
    }
  }

  tags = {
    Name = "${var.project_name}-ecs-instance"
    Type = "ECS"
  }

  lifecycle {
    create_before_destroy = true
  }
}

# Elastic IP (optional)
resource "aws_eip" "ec2_eip" {
  count    = var.associate_public_ip ? 1 : 0
  instance = aws_instance.ecs_instance.id
  domain   = "vpc"

  tags = {
    Name = "${var.project_name}-ec2-eip"
  }

  depends_on = [aws_instance.ecs_instance]
} 
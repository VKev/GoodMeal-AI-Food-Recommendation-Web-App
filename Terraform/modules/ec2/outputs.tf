output "instance_id" {
  description = "ID of the EC2 instance"
  value       = aws_instance.ecs_instance.id
}

output "instance_public_ip" {
  description = "Public IP address of the EC2 instance"
  value       = aws_instance.ecs_instance.public_ip
}

output "instance_private_ip" {
  description = "Private IP address of the EC2 instance"
  value       = aws_instance.ecs_instance.private_ip
}

output "instance_public_dns" {
  description = "Public DNS name of the EC2 instance"
  value       = aws_instance.ecs_instance.public_dns
}

output "elastic_ip" {
  description = "Elastic IP address associated with the instance (if created)"
  value       = var.associate_public_ip ? aws_eip.ec2_eip[0].public_ip : null
}

output "security_group_id" {
  description = "ID of the security group attached to the EC2 instance"
  value       = aws_security_group.ec2_sg.id
}

output "ecs_cluster_name" {
  description = "Name of the ECS cluster"
  value       = aws_ecs_cluster.main.name
}

output "ecs_cluster_arn" {
  description = "ARN of the ECS cluster"
  value       = aws_ecs_cluster.main.arn
}

output "iam_role_arn" {
  description = "ARN of the IAM role attached to the EC2 instance"
  value       = aws_iam_role.ec2_role.arn
}

output "iam_instance_profile_name" {
  description = "Name of the IAM instance profile"
  value       = aws_iam_instance_profile.ec2_profile.name
}

output "key_pair_name" {
  description = "Name of the key pair used for the EC2 instance"
  value       = aws_key_pair.ec2_key.key_name
}

output "ami_id" {
  description = "ID of the ECS-optimized AMI used"
  value       = data.aws_ami.ecs_optimized.id
} 
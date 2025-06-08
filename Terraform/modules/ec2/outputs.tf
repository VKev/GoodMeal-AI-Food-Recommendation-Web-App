#####################################################
#   EC2 INSTANCE DETAILS
#####################################################
output "instance_id" {
  description = "ID of the ECS container instance"
  value       = aws_instance.ecs_instance.id
}

output "instance_public_ip" {
  description = "Public IPv4 address (if associated)"
  value       = aws_instance.ecs_instance.public_ip
}

output "instance_private_ip" {
  description = "Private IPv4 address"
  value       = aws_instance.ecs_instance.private_ip
}

output "instance_public_dns" {
  description = "Public DNS name (if public IP associated)"
  value       = aws_instance.ecs_instance.public_dns
}

output "elastic_ip" {
  description = "Elastic IP attached to the instance (null if disabled)"
  value       = var.associate_public_ip ? aws_eip.ec2_eip[0].public_ip : null
}

output "security_group_id" {
  description = "Security-group ID applied to the instance"
  value       = aws_security_group.ec2_sg.id
}

#####################################################
#   ECS CLUSTER DETAILS
#####################################################
output "ecs_cluster_id" {
  description = "ID of the ECS cluster"
  value       = aws_ecs_cluster.main.id
}

output "ecs_cluster_name" {
  description = "Name of the ECS cluster"
  value       = aws_ecs_cluster.main.name
}

output "ecs_cluster_arn" {
  description = "ARN of the ECS cluster"
  value       = aws_ecs_cluster.main.arn
}

#####################################################
#   IAM / ACCESS
#####################################################
output "iam_role_arn" {
  description = "ARN of the IAM role attached to the instance"
  value       = aws_iam_role.ec2_role.arn
}

output "iam_instance_profile_name" {
  description = "Name of the IAM instance profile"
  value       = aws_iam_instance_profile.ec2_profile.name
}

output "key_pair_name" {
  description = "Key-pair name used for SSH"
  value       = aws_key_pair.ec2_key.key_name
}

#####################################################
#   AMI & KEY MATERIAL
#####################################################
output "ami_id" {
  description = "AMI ID of the Amazon Linux 2 ECS-optimised image"
  value       = data.aws_ami.ecs_optimized.id
}

output "ec2_private_key_pem" {
  description = "Private key material for the generated key pair"
  value       = tls_private_key.ec2_key.private_key_pem
  sensitive   = true
}

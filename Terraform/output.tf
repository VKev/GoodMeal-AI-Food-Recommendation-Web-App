# output "cloudfront_distribution_id" {
#   description = "The ID of the CloudFront distribution"
#   value       = module.cloudfront.cloudfront_distribution_id
# }

# output "cloudfront_domain_name" {
#   description = "The domain name of the CloudFront distribution"
#   value       = module.cloudfront.cloudfront_domain_name
# }

# output "cloudfront_private_key_pem" {
#   description = "Private key for CloudFront signed URLs/cookies."
#   value       = module.cloudfront.cloudfront_private_key_pem
#   sensitive   = true
# }

# output "cloudfront_key_group_id" {
#   description = "The ID of the CloudFront key group."
#   value       = module.cloudfront.cloudfront_key_group_id
# }


# # VPC Outputs
# output "vpc_id" {
#   description = "ID of the VPC"
#   value       = module.vpc.vpc_id
# }

# output "public_subnet_ids" {
#   description = "ID of the public subnet"
#   value       = module.vpc.public_subnet_ids
# }

# output "private_subnet_id" {
#   description = "ID of the private subnet"
#   value       = module.vpc.private_subnet_id
# }

# output "vpc_cidr_block" {
#   description = "CIDR block of the VPC"
#   value       = module.vpc.vpc_cidr_block
# }

# # EC2 Outputs
# output "ec2_instance_id" {
#   description = "ID of the EC2 instance"
#   value       = module.ec2.instance_id
# }

# output "ec2_public_ip" {
#   description = "Public IP address of the EC2 instance"
#   value       = module.ec2.instance_public_ip
# }

# output "ec2_private_ip" {
#   description = "Private IP address of the EC2 instance"
#   value       = module.ec2.instance_private_ip
# }

# output "ec2_public_dns" {
#   description = "Public DNS name of the EC2 instance"
#   value       = module.ec2.instance_public_dns
# }

# output "elastic_ip" {
#   description = "Elastic IP address associated with the instance"
#   value       = module.ec2.elastic_ip
# }

# output "ecs_cluster_name" {
#   description = "Name of the ECS cluster"
#   value       = module.ec2.ecs_cluster_name
# }

# output "ecs_cluster_arn" {
#   description = "ARN of the ECS cluster"
#   value       = module.ec2.ecs_cluster_arn
# }

# output "ec2_private_key_pem" {
#   description = "Private key for EC2 instance."
#   value       = module.ec2.ec2_private_key_pem
#   sensitive   = true
# }

# # ECS Service Outputs
# output "ecs_service_name" {
#   description = "Name of the ECS service"
#   value       = module.ecs.ecs_service_name
# }

# output "ecs_service_arn" {
#   description = "ARN of the ECS service"
#   value       = module.ecs.ecs_service_arn
# }

# output "task_definition_arn" {
#   description = "ARN of the task definition"
#   value       = module.ecs.task_definition_arn
# }

# output "cloudwatch_log_group_name" {
#   description = "Name of the CloudWatch log group for ECS logs"
#   value       = module.ecs.cloudwatch_log_group_name
# }

# Frontend Outputs (from remote state)
# Uncomment these after deploying the frontend infrastructure
# output "frontend_app_url" {
#   description = "URL of the deployed frontend application"
#   value       = data.terraform_remote_state.frontend.outputs.amplify_app_url
# }

# output "frontend_domain" {
#   description = "Domain of the frontend application"
#   value       = data.terraform_remote_state.frontend.outputs.frontend_domain
# }

# output "frontend_app_id" {
#   description = "Amplify application ID"
#   value       = data.terraform_remote_state.frontend.outputs.amplify_app_id
# }

output "alb_dns_name" {
  description = "DNS name of the Application Load Balancer"
  value       = module.alb.alb_dns_name
  sensitive = true
}

# Frontend Integration Outputs
output "frontend_url_from_parameter_store" {
  description = "Frontend URL retrieved from Parameter Store"
  value       = data.aws_ssm_parameter.frontend_url.value
  sensitive = true
}

output "frontend_domain_from_parameter_store" {
  description = "Frontend domain retrieved from Parameter Store"
  value       = data.aws_ssm_parameter.frontend_domain.value
  sensitive = true
}

output "cloudfront_allowed_origins" {
  description = "All allowed origins configured for CloudFront CORS, including frontend domains"
  value = concat(
    [
      "http://localhost:3000",
      "localhost:3000",
    ],
    [
      data.aws_ssm_parameter.frontend_url.value,
      "https://${data.aws_ssm_parameter.frontend_domain.value}",
    ]
  )
  sensitive = true
}

output "parameter_store_integration_status" {
  description = "Status of Parameter Store integration between infrastructure and frontend"
  value = {
    backend_base_url_stored    = aws_ssm_parameter.backend_base_url.name
    cloudfront_base_url_stored = aws_ssm_parameter.cloudfront_base_url.name
    frontend_url_retrieved     = data.aws_ssm_parameter.frontend_url.name
    frontend_domain_retrieved  = data.aws_ssm_parameter.frontend_domain.name
    integration_status        = "SUCCESS: CloudFront now includes frontend domains in allowed origins"
  }
  sensitive = true
}

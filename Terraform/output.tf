# output "cloudfront_distribution_id" {
#   description = "The ID of the CloudFront distribution"
#   value       = module.cloudfront.cloudfront_distribution_id
# }

# output "cloudfront_domain_name" {
#   description = "The domain name of the CloudFront distribution"
#   value       = module.cloudfront.cloudfront_domain_name
# }

# VPC Outputs
output "vpc_id" {
  description = "ID of the VPC"
  value       = module.vpc.vpc_id
}

output "public_subnet_id" {
  description = "ID of the public subnet"
  value       = module.vpc.public_subnet_id
}

output "private_subnet_id" {
  description = "ID of the private subnet"
  value       = module.vpc.private_subnet_id
}

output "vpc_cidr_block" {
  description = "CIDR block of the VPC"
  value       = module.vpc.vpc_cidr_block
}
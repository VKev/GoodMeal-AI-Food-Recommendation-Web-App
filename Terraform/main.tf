# VPC Module
module "vpc" {
  source = "./modules/vpc"
  
  project_name         = var.project_name
  vpc_cidr            = var.vpc_cidr
  public_subnet_cidr  = var.public_subnet_cidr
  private_subnet_cidr = var.private_subnet_cidr
}

# EC2 Module
module "ec2" {
  source = "./modules/ec2"
  
  project_name        = var.project_name
  vpc_id             = module.vpc.vpc_id
  vpc_cidr           = var.vpc_cidr
  subnet_id          = module.vpc.public_subnet_id  # Using public subnet for easier access
  instance_type      = var.instance_type
  associate_public_ip = var.associate_public_ip
}

# module "lambda_edge" {
#   source = "./modules/lambda_edge"
# }

# module "cloudfront" {
#   source                    = "./modules/cloudfront"
#   origin_domain_name        = var.origin_domain_name
#   origin_path               = var.origin_path
#   set_cookie_lambda_arn     = module.lambda_edge.lambda_function_qualified_arn
#   bucket_secret_referer     = var.bucket_secret_referer
# }
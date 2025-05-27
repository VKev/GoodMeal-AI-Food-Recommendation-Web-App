# VPC Module
module "vpc" {
  source = "./modules/vpc"
  
  project_name         = var.project_name
  vpc_cidr            = var.vpc_cidr
  public_subnet_cidr  = var.public_subnet_cidr
  private_subnet_cidr = var.private_subnet_cidr
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
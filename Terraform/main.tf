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

# ECS Module
module "ecs" {
  source = "./modules/ecs"
  
  project_name       = var.project_name
  aws_region        = var.aws_region
  vpc_id            = module.vpc.vpc_id
  ecs_cluster_id    = module.ec2.ecs_cluster_arn
  ecs_cluster_name  = module.ec2.ecs_cluster_name
  
  # Container configuration
  ecr_repository_url = "242201290212.dkr.ecr.us-east-1.amazonaws.com/goodmeal-ecr:Guest.Microservice-df6c4cfa2317f4bd693b4c6fed08d973e3ab47f1"
  image_tag         = "latest"
  container_name    = "goodmeal-guest-microservice"
  container_port    = 5001
  container_memory  = 1024
  container_cpu     = 1024
  desired_count     = 1
  

  
  enable_auto_scaling      = var.enable_auto_scaling
  enable_service_discovery = var.enable_service_discovery
  health_check_command = ["CMD-SHELL", "curl -f http://localhost:5001/ || exit 1"]
  depends_on = [module.ec2]
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
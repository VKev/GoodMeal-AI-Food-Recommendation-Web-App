module "vpc" {
  source                = "./modules/vpc"
  project_name          = var.project_name
  vpc_cidr              = var.vpc_cidr
  public_subnet_cidrs   = var.public_subnet_cidrs
  public_subnet_count   = 2
  private_subnet_cidr   = var.private_subnet_cidr
}

module "lambda_edge" {
  source = "./modules/lambda_edge"
  lambda_edge_secret = var.lambda_edge_secret
}

module "cloudfront" {
  source                = "./modules/cloudfront"
  project_name          = var.project_name
  origin_domain_name    = var.origin_domain_name
  origin_path           = var.origin_path
  set_cookie_lambda_arn = module.lambda_edge.lambda_function_qualified_arn
  bucket_secret_referer = var.bucket_secret_referer
}

# module "alb" {
#   source              = "./modules/alb"
#   project_name        = var.project_name
#   vpc_id              = module.vpc.vpc_id
#   public_subnet_ids   = module.vpc.public_subnet_ids

#   target_groups_definition = [
#     { 
#       name_suffix = "guest"
#       port        = var.services["guest"].alb_target_group_port
#       protocol    = var.services["guest"].alb_target_group_protocol
#       target_type = var.services["guest"].alb_target_group_type
#       health_check = {
#         enabled             = var.services["guest"].alb_health_check.enabled
#         path                = var.services["guest"].alb_health_check.path
#         port                = var.services["guest"].alb_health_check.port
#         protocol            = var.services["guest"].alb_health_check.protocol
#         matcher             = var.services["guest"].alb_health_check.matcher
#         interval            = var.services["guest"].alb_health_check.interval
#         timeout             = var.services["guest"].alb_health_check.timeout
#         healthy_threshold   = var.services["guest"].alb_health_check.healthy_threshold
#         unhealthy_threshold = var.services["guest"].alb_health_check.unhealthy_threshold
#       }
#     },
#     { 
#       name_suffix = "user"
#       port        = var.services["user"].alb_target_group_port
#       protocol    = var.services["user"].alb_target_group_protocol
#       target_type = var.services["user"].alb_target_group_type
#       health_check = {
#         enabled             = var.services["user"].alb_health_check.enabled
#         path                = var.services["user"].alb_health_check.path
#         port                = var.services["user"].alb_health_check.port
#         protocol            = var.services["user"].alb_health_check.protocol
#         matcher             = var.services["user"].alb_health_check.matcher
#         interval            = var.services["user"].alb_health_check.interval
#         timeout             = var.services["user"].alb_health_check.timeout
#         healthy_threshold   = var.services["user"].alb_health_check.healthy_threshold
#         unhealthy_threshold = var.services["user"].alb_health_check.unhealthy_threshold
#       }
#     },
#     { 
#       name_suffix = "resource"
#       port        = var.services["resource"].alb_target_group_port
#       protocol    = var.services["resource"].alb_target_group_protocol
#       target_type = var.services["resource"].alb_target_group_type
#       health_check = {
#         enabled             = var.services["resource"].alb_health_check.enabled
#         path                = var.services["resource"].alb_health_check.path
#         port                = var.services["resource"].alb_health_check.port
#         protocol            = var.services["resource"].alb_health_check.protocol
#         matcher             = var.services["resource"].alb_health_check.matcher
#         interval            = var.services["resource"].alb_health_check.interval
#         timeout             = var.services["resource"].alb_health_check.timeout
#         healthy_threshold   = var.services["resource"].alb_health_check.healthy_threshold
#         unhealthy_threshold = var.services["resource"].alb_health_check.unhealthy_threshold
#       }
#     }
#   ]

#   default_listener_action = {
#     type = "fixed-response"
#     fixed_response = {
#       content_type = "text/plain"
#       message_body = "Error: Path not found."
#       status_code  = "404"
#     }
#   }

#   listener_rules_definition = [
#     { 
#       priority            = var.services["guest"].alb_listener_rule_priority
#       target_group_suffix = "guest"
#       conditions          = var.services["guest"].alb_listener_rule_conditions
#     },
#     { 
#       priority            = var.services["user"].alb_listener_rule_priority
#       target_group_suffix = "user"
#       conditions          = var.services["user"].alb_listener_rule_conditions
#     },
#     {
#       priority            = var.services["resource"].alb_listener_rule_priority
#       target_group_suffix = "resource"
#       conditions          = var.services["resource"].alb_listener_rule_conditions
#     }
#   ]
# }

# # EC2 Module
# module "ec2" {
#   source                = "./modules/ec2"
#   project_name          = var.project_name
#   vpc_id                = module.vpc.vpc_id
#   vpc_cidr              = var.vpc_cidr
#   subnet_id             = module.vpc.public_subnet_ids[0]
#   instance_type         = var.instance_type
#   associate_public_ip   = var.associate_public_ip
#   alb_security_group_id = module.alb.alb_sg_id

#   depends_on = [module.alb]
# }

# # ECS Module
# module "ecs" {
#   source = "./modules/ecs"

#   project_name     = var.project_name
#   aws_region       = var.aws_region
#   vpc_id           = module.vpc.vpc_id
#   ecs_cluster_id   = module.ec2.ecs_cluster_arn
#   ecs_cluster_name = module.ec2.ecs_cluster_name
#   desired_count    = 1

#   task_cpu    = 825
#   task_memory = 825

#   containers = [
#     { 
#       name                 = "${var.project_name}-guest-${var.services["guest"].ecs_container_name_suffix}"
#       image_repository_url = var.services["guest"].ecs_container_image_repository_url
#       image_tag            = var.services["guest"].ecs_container_image_tag
#       cpu                  = var.services["guest"].ecs_container_cpu
#       memory               = var.services["guest"].ecs_container_memory
#       essential            = var.services["guest"].ecs_container_essential
#       port_mappings        = var.services["guest"].ecs_container_port_mappings
#       environment_variables= var.services["guest"].ecs_environment_variables
#       health_check = {
#         command     = var.services["guest"].ecs_container_health_check.command
#         interval    = var.services["guest"].ecs_container_health_check.interval
#         timeout     = var.services["guest"].ecs_container_health_check.timeout
#         retries     = var.services["guest"].ecs_container_health_check.retries
#         startPeriod = var.services["guest"].ecs_container_health_check.startPeriod
#       }
#       enable_service_discovery = var.enable_service_discovery # Uses the global variable
#       service_discovery_port   = var.services["guest"].ecs_service_discovery_port
#     },
#     { 
#       name                 = "${var.project_name}-user-${var.services["user"].ecs_container_name_suffix}"
#       image_repository_url = var.services["user"].ecs_container_image_repository_url
#       image_tag            = var.services["user"].ecs_container_image_tag
#       cpu                  = var.services["user"].ecs_container_cpu
#       memory               = var.services["user"].ecs_container_memory
#       essential            = var.services["user"].ecs_container_essential
#       port_mappings        = var.services["user"].ecs_container_port_mappings
#       environment_variables= var.services["user"].ecs_environment_variables
#       health_check = {
#         command     = var.services["user"].ecs_container_health_check.command
#         interval    = var.services["user"].ecs_container_health_check.interval
#         timeout     = var.services["user"].ecs_container_health_check.timeout
#         retries     = var.services["user"].ecs_container_health_check.retries
#         startPeriod = var.services["user"].ecs_container_health_check.startPeriod
#       }
#       enable_service_discovery = var.enable_service_discovery # Uses the global variable
#       service_discovery_port   = var.services["user"].ecs_service_discovery_port
#     },
#     { 
#       name                 = "${var.project_name}-resource-${var.services["resource"].ecs_container_name_suffix}"
#       image_repository_url = var.services["resource"].ecs_container_image_repository_url
#       image_tag            = var.services["resource"].ecs_container_image_tag
#       cpu                  = var.services["resource"].ecs_container_cpu
#       memory               = var.services["resource"].ecs_container_memory
#       essential            = var.services["resource"].ecs_container_essential
#       port_mappings        = var.services["resource"].ecs_container_port_mappings
#       environment_variables= concat(
#         var.services["resource"].ecs_environment_variables,
#         [
#           {
#             name  = "AWS_CLOUD_FRONT_KEY_ID"
#             value = module.cloudfront.public_key_encoded
#           },
#           {
#             name  = "AWS_CLOUD_FRONT_PRIVATE_KEY"
#             value = module.cloudfront.cloudfront_private_key_pem
#           },
#           {
#             name  = "AWS_CLOUD_FRONT_DISTRIBUTION_DOMAIN"
#             value = module.cloudfront.cloudfront_domain_name
#           }
#         ]
#       )
#       health_check = {
#         command     = var.services["resource"].ecs_container_health_check.command
#         interval    = var.services["resource"].ecs_container_health_check.interval
#         timeout     = var.services["resource"].ecs_container_health_check.timeout
#         retries     = var.services["resource"].ecs_container_health_check.retries
#         startPeriod = var.services["resource"].ecs_container_health_check.startPeriod
#       }
#       enable_service_discovery = var.enable_service_discovery # Uses the global variable
#       service_discovery_port   = var.services["resource"].ecs_service_discovery_port
#     }
#   ]

#   target_groups = [
#     { 
#       target_group_arn = module.alb.target_group_arns_map["guest"]
#       container_name   = "${var.project_name}-guest-${var.services["guest"].ecs_container_name_suffix}"
#       container_port   = var.services["guest"].ecs_container_port_mappings[0].container_port # Assumes 
#     },
#     {
#       target_group_arn = module.alb.target_group_arns_map["user"]
#       container_name   = "${var.project_name}-user-${var.services["user"].ecs_container_name_suffix}"
#       container_port   = var.services["user"].ecs_container_port_mappings[0].container_port # Assumes 
#     },
#     {
#       target_group_arn = module.alb.target_group_arns_map["resource"]
#       container_name   = "${var.project_name}-resource-${var.services["resource"].ecs_container_name_suffix}"
#       container_port   = var.services["resource"].ecs_container_port_mappings[0].container_port # Assumes 
#     }
#   ]

#   enable_auto_scaling      = var.enable_auto_scaling
#   enable_service_discovery = var.enable_service_discovery

#   depends_on = [module.ec2]
# }

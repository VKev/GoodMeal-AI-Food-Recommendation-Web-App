module "vpc" {
  source                = "./modules/vpc"
  project_name          = var.project_name
  vpc_cidr              = var.vpc_cidr
  public_subnet_cidrs   = var.public_subnet_cidrs
  public_subnet_count   = 2
  private_subnet_cidr   = var.private_subnet_cidr
}

# Frontend Infrastructure (Amplify) is managed separately in ./frontend/ directory
# Uncomment the data source below to consume frontend outputs in this configuration
# data "terraform_remote_state" "frontend" {
#   backend = "s3"
#   config = {
#     bucket = "khangstoragetest"
#     key    = "terraform/frontend/state.tfstate"
#     region = "ap-southeast-1"
#     endpoints = {
#       s3 = "https://s3.ap-southeast-1.wasabisys.com"
#     }
#     profile                     = "wasabi-user"
#     use_path_style              = true
#     skip_credentials_validation = true
#     skip_requesting_account_id  = true
#   }
# }

# Example of how to use frontend outputs:
# frontend_app_url = data.terraform_remote_state.frontend.outputs.amplify_app_url
# frontend_domain  = data.terraform_remote_state.frontend.outputs.frontend_domain


module "lambda_edge" {
  source = "./modules/lambda_edge"
  lambda_edge_secret = var.lambda_edge_secret
}

# Data sources to get frontend information from Parameter Store
data "aws_ssm_parameter" "frontend_url" {
  name = "/${var.project_name}/frontend/app_url"
}

data "aws_ssm_parameter" "frontend_domain" {
  name = "/${var.project_name}/frontend/domain"
}

module "cloudfront" {
  source                = "./modules/cloudfront"
  project_name          = var.project_name
  origin_domain_name    = var.origin_domain_name
  origin_path           = var.origin_path
  set_cookie_lambda_arn = module.lambda_edge.lambda_function_qualified_arn
  bucket_secret_referer = var.bucket_secret_referer
  cloudfront_allow_origins = concat(
    [
      "http://localhost:3000",
      "localhost:3000",
    ],
    [
      data.aws_ssm_parameter.frontend_url.value,
      "https://${data.aws_ssm_parameter.frontend_domain.value}",
    ]
  )
}

module "alb" {
  source              = "./modules/alb"
  project_name        = var.project_name
  vpc_id              = module.vpc.vpc_id
  public_subnet_ids   = module.vpc.public_subnet_ids

  target_groups_definition = [
    { 
      name_suffix = "api-gateway"
      port        = var.services["api-gateway"].alb_target_group_port
      protocol    = var.services["api-gateway"].alb_target_group_protocol
      target_type = var.services["api-gateway"].alb_target_group_type
      health_check = {
        enabled             = var.services["api-gateway"].alb_health_check.enabled
        path                = var.services["api-gateway"].alb_health_check.path
        port                = var.services["api-gateway"].alb_health_check.port
        protocol            = var.services["api-gateway"].alb_health_check.protocol
        matcher             = var.services["api-gateway"].alb_health_check.matcher
        interval            = var.services["api-gateway"].alb_health_check.interval
        timeout             = var.services["api-gateway"].alb_health_check.timeout
        healthy_threshold   = var.services["api-gateway"].alb_health_check.healthy_threshold
        unhealthy_threshold = var.services["api-gateway"].alb_health_check.unhealthy_threshold
      }
    }
  ]

  default_listener_action = {
    type = "fixed-response"
    fixed_response = {
      content_type = "text/plain"
      message_body = "Error: Path not found."
      status_code  = "404"
    }
  }

  listener_rules_definition = [
    {
      priority            = var.services["api-gateway"].alb_listener_rule_priority
      target_group_suffix = "api-gateway"
      conditions          = var.services["api-gateway"].alb_listener_rule_conditions
    }
  ]
}

# EC2 Module
module "ec2" {
  source                = "./modules/ec2"
  project_name          = var.project_name
  vpc_id                = module.vpc.vpc_id
  vpc_cidr              = var.vpc_cidr
  subnet_id             = module.vpc.public_subnet_ids[0]
  instance_type         = var.instance_type
  associate_public_ip   = var.associate_public_ip
  alb_security_group_id = module.alb.alb_sg_id

  depends_on = [module.alb]
}

# ECS Module
module "ecs" {
  source = "./modules/ecs"

  project_name     = var.project_name
  aws_region       = var.aws_region
  vpc_id           = module.vpc.vpc_id
  ecs_cluster_id   = module.ec2.ecs_cluster_arn
  ecs_cluster_name = module.ec2.ecs_cluster_name
  desired_count    = 1

  task_cpu    = 850
  task_memory = 850

  containers = [
    { 
      name                 = "${var.project_name}-guest-${var.services["guest"].ecs_container_name_suffix}"
      image_repository_url = var.services["guest"].ecs_container_image_repository_url
      image_tag            = var.services["guest"].ecs_container_image_tag
      cpu                  = var.services["guest"].ecs_container_cpu
      memory               = var.services["guest"].ecs_container_memory
      essential            = var.services["guest"].ecs_container_essential
      port_mappings        = var.services["guest"].ecs_container_port_mappings
      environment_variables= concat(
        var.services["guest"].ecs_environment_variables,
        [
          {
            name  = "CORS_ALLOWED_ORIGINS"
            value = "${data.aws_ssm_parameter.frontend_url.value},https://${data.aws_ssm_parameter.frontend_domain.value}"
          },
          {
            name  = "CORS_ALLOW_CREDENTIALS"
            value = "true"
          }
        ]
      )
      health_check = {
        command     = var.services["guest"].ecs_container_health_check.command
        interval    = var.services["guest"].ecs_container_health_check.interval
        timeout     = var.services["guest"].ecs_container_health_check.timeout
        retries     = var.services["guest"].ecs_container_health_check.retries
        startPeriod = var.services["guest"].ecs_container_health_check.startPeriod
      }
      enable_service_discovery = var.enable_service_discovery # Uses the global variable
      service_discovery_port   = var.services["guest"].ecs_service_discovery_port
    },
    { 
      name                 = "${var.project_name}-user-${var.services["user"].ecs_container_name_suffix}"
      image_repository_url = var.services["user"].ecs_container_image_repository_url
      image_tag            = var.services["user"].ecs_container_image_tag
      cpu                  = var.services["user"].ecs_container_cpu
      memory               = var.services["user"].ecs_container_memory
      essential            = var.services["user"].ecs_container_essential
      port_mappings        = var.services["user"].ecs_container_port_mappings
      environment_variables= concat(
        var.services["user"].ecs_environment_variables,
        [
          {
            name  = "CORS_ALLOWED_ORIGINS"
            value = "${data.aws_ssm_parameter.frontend_url.value},https://${data.aws_ssm_parameter.frontend_domain.value}"
          },
          {
            name  = "CORS_ALLOW_CREDENTIALS"
            value = "true"
          }
        ]
      )
      health_check = {
        command     = var.services["user"].ecs_container_health_check.command
        interval    = var.services["user"].ecs_container_health_check.interval
        timeout     = var.services["user"].ecs_container_health_check.timeout
        retries     = var.services["user"].ecs_container_health_check.retries
        startPeriod = var.services["user"].ecs_container_health_check.startPeriod
      }
      enable_service_discovery = var.enable_service_discovery # Uses the global variable
      service_discovery_port   = var.services["user"].ecs_service_discovery_port
    },
    { 
      name                 = "${var.project_name}-resource-${var.services["resource"].ecs_container_name_suffix}"
      image_repository_url = var.services["resource"].ecs_container_image_repository_url
      image_tag            = var.services["resource"].ecs_container_image_tag
      cpu                  = var.services["resource"].ecs_container_cpu
      memory               = var.services["resource"].ecs_container_memory
      essential            = var.services["resource"].ecs_container_essential
      port_mappings        = var.services["resource"].ecs_container_port_mappings
      environment_variables= concat(
        var.services["resource"].ecs_environment_variables,
        [
          {
            name  = "CORS_ALLOWED_ORIGINS"
            value = "${data.aws_ssm_parameter.frontend_url.value},https://${data.aws_ssm_parameter.frontend_domain.value}"
          },
          {
            name  = "CORS_ALLOW_CREDENTIALS"
            value = "true"
          }
        ]
      )
      health_check = {
        command     = var.services["resource"].ecs_container_health_check.command
        interval    = var.services["resource"].ecs_container_health_check.interval
        timeout     = var.services["resource"].ecs_container_health_check.timeout
        retries     = var.services["resource"].ecs_container_health_check.retries
        startPeriod = var.services["resource"].ecs_container_health_check.startPeriod
      }
      enable_service_discovery = var.enable_service_discovery # Uses the global variable
      service_discovery_port   = var.services["resource"].ecs_service_discovery_port
    },
    { 
      name                 = "${var.project_name}-api-gateway-${var.services["api-gateway"].ecs_container_name_suffix}"
      image_repository_url = var.services["api-gateway"].ecs_container_image_repository_url
      image_tag            = var.services["api-gateway"].ecs_container_image_tag
      cpu                  = var.services["api-gateway"].ecs_container_cpu
      memory               = var.services["api-gateway"].ecs_container_memory
      essential            = var.services["api-gateway"].ecs_container_essential
      port_mappings        = var.services["api-gateway"].ecs_container_port_mappings
      environment_variables= concat(
        [
          {
            name  = "ASPNETCORE_ENVIRONMENT"
            value = "Production"
          },
          {
            name  = "ASPNETCORE_URLS"
            value = "http://+:8080"
          },
          {
            name  = "OCELOT_GLOBAL_BASE_URL"
            value = "http://localhost:2406"
          },
          {
            name  = "CORS_ALLOWED_ORIGINS"
            value = "${data.aws_ssm_parameter.frontend_url.value},https://${data.aws_ssm_parameter.frontend_domain.value}"
          },
          {
            name  = "CORS_ALLOW_CREDENTIALS"
            value = "true"
          }
        ],
        [
          {
            name  = "OCELOT_ROUTES_0_UPSTREAM_PATH"
            value = "/api/User/{everything}"
          },
          {
            name  = "OCELOT_ROUTES_0_UPSTREAM_METHODS"
            value = "Get,Post,Put,Delete"
          },
          {
            name  = "OCELOT_ROUTES_0_DOWNSTREAM_SCHEME"
            value = "http"
          },
          {
            name  = "OCELOT_ROUTES_0_DOWNSTREAM_HOST"
            value = "${var.project_name}-user-${var.services["user"].ecs_container_name_suffix}"
          },
          {
            name  = "OCELOT_ROUTES_0_DOWNSTREAM_PORT"
            value = "${var.services["user"].ecs_service_discovery_port}"
          },
          {
            name  = "OCELOT_ROUTES_0_DOWNSTREAM_PATH"
            value = "/api/User/{everything}"
          },
          {
            name  = "OCELOT_ROUTES_1_UPSTREAM_PATH"
            value = "/api/Resource/{everything}"
          },
          {
            name  = "OCELOT_ROUTES_1_UPSTREAM_METHODS"
            value = "Get,Post,Put,Delete"
          },
          {
            name  = "OCELOT_ROUTES_1_DOWNSTREAM_SCHEME"
            value = "http"
          },
          {
            name  = "OCELOT_ROUTES_1_DOWNSTREAM_HOST"
            value = "${var.project_name}-resource-${var.services["resource"].ecs_container_name_suffix}"
          },
          {
            name  = "OCELOT_ROUTES_1_DOWNSTREAM_PORT"
            value = "${var.services["resource"].ecs_service_discovery_port}"
          },
          {
            name  = "OCELOT_ROUTES_1_DOWNSTREAM_PATH"
            value = "/api/Resource/{everything}"
          },
          {
            name  = "OCELOT_ROUTES_2_UPSTREAM_PATH"
            value = "/api/Guest/{everything}"
          },
          {
            name  = "OCELOT_ROUTES_2_UPSTREAM_METHODS"
            value = "Get,Post,Put,Delete"
          },
          {
            name  = "OCELOT_ROUTES_2_DOWNSTREAM_SCHEME"
            value = "http"
          },
          {
            name  = "OCELOT_ROUTES_2_DOWNSTREAM_HOST"
            value = "${var.project_name}-guest-${var.services["guest"].ecs_container_name_suffix}"
          },
          {
            name  = "OCELOT_ROUTES_2_DOWNSTREAM_PORT"
            value = "${var.services["guest"].ecs_service_discovery_port}"
          },
          {
            name  = "OCELOT_ROUTES_2_DOWNSTREAM_PATH"
            value = "/api/Guest/{everything}"
          },
        ]
      )
      health_check = {
        command     = var.services["api-gateway"].ecs_container_health_check.command
        interval    = var.services["api-gateway"].ecs_container_health_check.interval
        timeout     = var.services["api-gateway"].ecs_container_health_check.timeout
        retries     = var.services["api-gateway"].ecs_container_health_check.retries
        startPeriod = var.services["api-gateway"].ecs_container_health_check.startPeriod
      }
      enable_service_discovery = var.enable_service_discovery # Uses the global variable
      service_discovery_port   = var.services["api-gateway"].ecs_service_discovery_port
    }
  ]

  target_groups = [
    {
      target_group_arn = module.alb.target_group_arns_map["api-gateway"]
      container_name   = "${var.project_name}-api-gateway-${var.services["api-gateway"].ecs_container_name_suffix}"
      container_port   = var.services["api-gateway"].ecs_container_port_mappings[0].container_port
    }
  ]

  enable_auto_scaling      = var.enable_auto_scaling
  enable_service_discovery = var.enable_service_discovery

  depends_on = [module.ec2]
}

# Parameter Store - Store infrastructure outputs for frontend consumption
# These parameters will be automatically picked up by the frontend module

# Store ALB DNS name as backend base URL
resource "aws_ssm_parameter" "backend_base_url" {
  name        = "/${var.project_name}/backend/base_url"
  description = "Backend base URL (ALB DNS name) for frontend consumption"
  type        = "String"
  value       = "https://${module.alb.alb_dns_name}"

  tags = {
    Project     = var.project_name
    Environment = "production"
    Service     = "infrastructure"
    Purpose     = "frontend-integration"
  }
}

# Store CloudFront domain as frontend CDN base URL
resource "aws_ssm_parameter" "cloudfront_base_url" {
  name        = "/${var.project_name}/cloudfront/base_url"
  description = "CloudFront base URL for frontend static assets"
  type        = "String"
  value       = "https://${module.cloudfront.cloudfront_domain_name}"

  tags = {
    Project     = var.project_name
    Environment = "production"
    Service     = "infrastructure"
    Purpose     = "frontend-integration"
  }
}

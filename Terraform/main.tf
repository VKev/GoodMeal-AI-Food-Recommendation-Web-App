module "vpc" {
  source                = "./modules/vpc"
  project_name          = var.project_name
  vpc_cidr              = var.vpc_cidr
  public_subnet_cidrs   = var.public_subnet_cidrs
  public_subnet_count   = 2
  private_subnet_cidr   = var.private_subnet_cidr
}

# Amplify Module for Next.js Frontend
# module "amplify" {
#   source = "./modules/amplify"
  
#   app_name                      = var.project_name
#   github_repository_url         = var.amplify_github_repository_url
#   github_access_token          = var.amplify_github_access_token
#   branch_name                  = var.amplify_branch_name
#   environment_variables        = merge(
#     var.amplify_environment_variables,
#     {
#       TEST_VAR = "TEST_VAR"
#       # BACKEND_BASE_URL = "https://${module.alb.alb_dns_name}"
#       # NEXT_PUBLIC_CLOUDFRONT_BASE_URL = "https://${module.cloudfront.cloudfront_domain_name}"
#     }
#   )
#   build_spec                   = var.amplify_build_spec
#   enable_auto_branch_creation  = var.amplify_enable_auto_branch_creation
#   enable_branch_auto_build     = var.amplify_enable_branch_auto_build
#   enable_branch_auto_deletion  = var.amplify_enable_branch_auto_deletion
#   framework                    = var.amplify_framework
#   rendering_mode               = var.amplify_rendering_mode
  
#   tags = {
#     Project     = var.project_name
#     Environment = "production"
#     Service     = "frontend"
#   }
#   # depends_on = [ module.cloudfront ]
# }


# module "lambda_edge" {
#   source = "./modules/lambda_edge"
#   lambda_edge_secret = var.lambda_edge_secret
# }

# module "cloudfront" {
#   source                = "./modules/cloudfront"
#   project_name          = var.project_name
#   origin_domain_name    = var.origin_domain_name
#   origin_path           = var.origin_path
#   set_cookie_lambda_arn = module.lambda_edge.lambda_function_qualified_arn
#   bucket_secret_referer = var.bucket_secret_referer
#   cloudfront_allow_origins = concat(
#     [
#       "http://localhost:3000",
#       "localhost:3000",
#       # module.amplify.app_url
#     ],
#   )
# }

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
      environment_variables= var.services["guest"].ecs_environment_variables
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
      environment_variables= var.services["user"].ecs_environment_variables
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
          # {
          #   name  = "AWS_CLOUD_FRONT_KEY_ID"
          #   value = module.cloudfront.public_key_encoded
          # },
          # {
          #   name  = "AWS_CLOUD_FRONT_PRIVATE_KEY"
          #   value = module.cloudfront.cloudfront_private_key_pem
          # },
          # {
          #   name  = "AWS_CLOUD_FRONT_DISTRIBUTION_DOMAIN"
          #   value = module.cloudfront.cloudfront_domain_name
          # }
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
          }
        ],
        # Ocelot route configurations - using service discovery names for internal communication
        [
          # User Service Routes
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
          # Auth Service Routes (Authentication Microservice)
          # {
          #   name  = "OCELOT_ROUTES_1_UPSTREAM_PATH"
          #   value = "/api/Auth/{everything}"
          # },
          # {
          #   name  = "OCELOT_ROUTES_1_UPSTREAM_METHODS"
          #   value = "Get,Post,Put,Delete"
          # },
          # {
          #   name  = "OCELOT_ROUTES_1_DOWNSTREAM_SCHEME"
          #   value = "http"
          # },
          # {
          #   name  = "OCELOT_ROUTES_1_DOWNSTREAM_HOST"
          #   value = "${var.project_name}-authentication-${var.services["authentication"].ecs_container_name_suffix}"
          # },
          # {
          #   name  = "OCELOT_ROUTES_1_DOWNSTREAM_PORT"
          #   value = "${var.services["authentication"].ecs_service_discovery_port}"
          # },
          # {
          #   name  = "OCELOT_ROUTES_1_DOWNSTREAM_PATH"
          #   value = "/api/Auth/{everything}"
          # },
          # Resource Service Routes
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
          # Guest Service Routes
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
          # Prompt Session Routes (Prompt Microservice)
          # {
          #   name  = "OCELOT_ROUTES_4_UPSTREAM_PATH"
          #   value = "/api/PromptSession/{everything}"
          # },
          # {
          #   name  = "OCELOT_ROUTES_4_UPSTREAM_METHODS"
          #   value = "Get,Post,Put,Delete"
          # },
          # {
          #   name  = "OCELOT_ROUTES_4_DOWNSTREAM_SCHEME"
          #   value = "http"
          # },
          # {
          #   name  = "OCELOT_ROUTES_4_DOWNSTREAM_HOST"
          #   value = "${var.project_name}-prompt-${var.services["prompt"].ecs_container_name_suffix}"
          # },
          # {
          #   name  = "OCELOT_ROUTES_4_DOWNSTREAM_PORT"
          #   value = "${var.services["prompt"].ecs_service_discovery_port}"
          # },
          # {
          #   name  = "OCELOT_ROUTES_4_DOWNSTREAM_PATH"
          #   value = "/api/PromptSession/{everything}"
          # },
          # # Message Routes (Prompt Microservice)
          # {
          #   name  = "OCELOT_ROUTES_5_UPSTREAM_PATH"
          #   value = "/api/Message/{everything}"
          # },
          # {
          #   name  = "OCELOT_ROUTES_5_UPSTREAM_METHODS"
          #   value = "Get,Post,Put,Delete"
          # },
          # {
          #   name  = "OCELOT_ROUTES_5_DOWNSTREAM_SCHEME"
          #   value = "http"
          # },
          # {
          #   name  = "OCELOT_ROUTES_5_DOWNSTREAM_HOST"
          #   value = "${var.project_name}-prompt-${var.services["prompt"].ecs_container_name_suffix}"
          # },
          # {
          #   name  = "OCELOT_ROUTES_5_DOWNSTREAM_PORT"
          #   value = "${var.services["prompt"].ecs_service_discovery_port}"
          # },
          # {
          #   name  = "OCELOT_ROUTES_5_DOWNSTREAM_PATH"
          #   value = "/api/Message/{everything}"
          # },
          # # Gemini Routes (Prompt Microservice)
          # {
          #   name  = "OCELOT_ROUTES_6_UPSTREAM_PATH"
          #   value = "/api/Gemini/{everything}"
          # },
          # {
          #   name  = "OCELOT_ROUTES_6_UPSTREAM_METHODS"
          #   value = "Get,Post,Put,Delete"
          # },
          # {
          #   name  = "OCELOT_ROUTES_6_DOWNSTREAM_SCHEME"
          #   value = "http"
          # },
          # {
          #   name  = "OCELOT_ROUTES_6_DOWNSTREAM_HOST"
          #   value = "${var.project_name}-prompt-${var.services["prompt"].ecs_container_name_suffix}"
          # },
          # {
          #   name  = "OCELOT_ROUTES_6_DOWNSTREAM_PORT"
          #   value = "${var.services["prompt"].ecs_service_discovery_port}"
          # },
          # {
          #   name  = "OCELOT_ROUTES_6_DOWNSTREAM_PATH"
          #   value = "/api/Gemini/{everything}"
          # },
          # # Restaurant Routes (Restaurant Microservice)
          # {
          #   name  = "OCELOT_ROUTES_7_UPSTREAM_PATH"
          #   value = "/api/Restaurant/{everything}"
          # },
          # {
          #   name  = "OCELOT_ROUTES_7_UPSTREAM_METHODS"
          #   value = "Get,Post,Put,Delete"
          # },
          # {
          #   name  = "OCELOT_ROUTES_7_DOWNSTREAM_SCHEME"
          #   value = "http"
          # },
          # {
          #   name  = "OCELOT_ROUTES_7_DOWNSTREAM_HOST"
          #   value = "${var.project_name}-restaurant-${var.services["restaurant"].ecs_container_name_suffix}"
          # },
          # {
          #   name  = "OCELOT_ROUTES_7_DOWNSTREAM_PORT"
          #   value = "${var.services["restaurant"].ecs_service_discovery_port}"
          # },
          # {
          #   name  = "OCELOT_ROUTES_7_DOWNSTREAM_PATH"
          #   value = "/api/Restaurant/{everything}"
          # },
          # # Food Routes (Restaurant Microservice)
          # {
          #   name  = "OCELOT_ROUTES_8_UPSTREAM_PATH"
          #   value = "/api/Food/{everything}"
          # },
          # {
          #   name  = "OCELOT_ROUTES_8_UPSTREAM_METHODS"
          #   value = "Get,Post,Put,Delete"
          # },
          # {
          #   name  = "OCELOT_ROUTES_8_DOWNSTREAM_SCHEME"
          #   value = "http"
          # },
          # {
          #   name  = "OCELOT_ROUTES_8_DOWNSTREAM_HOST"
          #   value = "${var.project_name}-restaurant-${var.services["restaurant"].ecs_container_name_suffix}"
          # },
          # {
          #   name  = "OCELOT_ROUTES_8_DOWNSTREAM_PORT"
          #   value = "${var.services["restaurant"].ecs_service_discovery_port}"
          # },
          # {
          #   name  = "OCELOT_ROUTES_8_DOWNSTREAM_PATH"
          #   value = "/api/Food/{everything}"
          # },
          # # Rating Routes (Restaurant Microservice)
          # {
          #   name  = "OCELOT_ROUTES_9_UPSTREAM_PATH"
          #   value = "/api/Rating/{everything}"
          # },
          # {
          #   name  = "OCELOT_ROUTES_9_UPSTREAM_METHODS"
          #   value = "Get,Post,Put,Delete"
          # },
          # {
          #   name  = "OCELOT_ROUTES_9_DOWNSTREAM_SCHEME"
          #   value = "http"
          # },
          # {
          #   name  = "OCELOT_ROUTES_9_DOWNSTREAM_HOST"
          #   value = "${var.project_name}-restaurant-${var.services["restaurant"].ecs_container_name_suffix}"
          # },
          # {
          #   name  = "OCELOT_ROUTES_9_DOWNSTREAM_PORT"
          #   value = "${var.services["restaurant"].ecs_service_discovery_port}"
          # },
          # {
          #   name  = "OCELOT_ROUTES_9_DOWNSTREAM_PATH"
          #   value = "/api/Rating/{everything}"
          # },
          # # Business Routes (Business Microservice)
          # {
          #   name  = "OCELOT_ROUTES_10_UPSTREAM_PATH"
          #   value = "/api/Business/{everything}"
          # },
          # {
          #   name  = "OCELOT_ROUTES_10_UPSTREAM_METHODS"
          #   value = "Get,Post,Put,Delete"
          # },
          # {
          #   name  = "OCELOT_ROUTES_10_DOWNSTREAM_SCHEME"
          #   value = "http"
          # },
          # {
          #   name  = "OCELOT_ROUTES_10_DOWNSTREAM_HOST"
          #   value = "${var.project_name}-business-${var.services["business"].ecs_container_name_suffix}"
          # },
          # {
          #   name  = "OCELOT_ROUTES_10_DOWNSTREAM_PORT"
          #   value = "${var.services["business"].ecs_service_discovery_port}"
          # },
          # {
          #   name  = "OCELOT_ROUTES_10_DOWNSTREAM_PATH"
          #   value = "/api/Business/{everything}"
          # },
          # # Admin Routes (Admin Microservice)
          # {
          #   name  = "OCELOT_ROUTES_11_UPSTREAM_PATH"
          #   value = "/api/Admin/{everything}"
          # },
          # {
          #   name  = "OCELOT_ROUTES_11_UPSTREAM_METHODS"
          #   value = "Get,Post,Put,Delete"
          # },
          # {
          #   name  = "OCELOT_ROUTES_11_DOWNSTREAM_SCHEME"
          #   value = "http"
          # },
          # {
          #   name  = "OCELOT_ROUTES_11_DOWNSTREAM_HOST"
          #   value = "${var.project_name}-admin-${var.services["admin"].ecs_container_name_suffix}"
          # },
          # {
          #   name  = "OCELOT_ROUTES_11_DOWNSTREAM_PORT"
          #   value = "${var.services["admin"].ecs_service_discovery_port}"
          # },
          # {
          #   name  = "OCELOT_ROUTES_11_DOWNSTREAM_PATH"
          #   value = "/api/Admin/{everything}"
          # }
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

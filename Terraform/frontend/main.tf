# Data source to get root infrastructure outputs (if needed)
# data "terraform_remote_state" "main" {
#   backend = "s3"
#   config = {
#     bucket = "khangstoragetest"
#     key    = "terraform/state.tfstate"
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

# Local values to safely try to get parameters from AWS Parameter Store
locals {
  # Only try to fetch parameters if both parameter store is enabled AND fetch is enabled
  should_fetch_parameters = var.enable_parameter_store && var.fetch_parameters_from_store
  
  # Create a map of parameter names to values (only for existing parameters)
  parameter_values = local.should_fetch_parameters ? {
    for param in var.parameter_store_parameters :
    param.env_var => try(
      data.aws_ssm_parameter.parameters["${param.name}"].value,
      null
    )
  } : {}
  
  # Filter out null values and create the final environment variables map
  parameter_env_vars = {
    for k, v in local.parameter_values :
    k => v if v != null
  }
}

# Data source to get parameters from AWS Parameter Store (if they exist)
# This will only fetch parameters when both conditions are true:
# 1. enable_parameter_store = true
# 2. fetch_parameters_from_store = true
data "aws_ssm_parameter" "parameters" {
  for_each = local.should_fetch_parameters ? {
    for param in var.parameter_store_parameters :
    param.name => param
  } : {}
  
  name            = "/${var.project_name}/${each.key}"
  with_decryption = each.value.decrypt
}

# Amplify Module for Next.js Frontend
module "amplify" {
  source = "../modules/amplify"
  
  app_name                      = var.project_name
  github_repository_url         = var.amplify_github_repository_url
  github_access_token          = var.amplify_github_access_token
  branch_name                  = var.amplify_branch_name
  environment_variables        = merge(
    var.amplify_environment_variables,
    local.parameter_env_vars,
  )
  build_spec                   = var.amplify_build_spec
  enable_auto_branch_creation  = var.amplify_enable_auto_branch_creation
  enable_branch_auto_build     = var.amplify_enable_branch_auto_build
  enable_branch_auto_deletion  = var.amplify_enable_branch_auto_deletion
  framework                    = var.amplify_framework
  rendering_mode               = var.amplify_rendering_mode
  trigger_initial_deployment   = var.trigger_initial_deployment
  
  tags = {
    Project     = var.project_name
    Environment = "production"
    Service     = "frontend"
  }
} 
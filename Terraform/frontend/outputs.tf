# Amplify App Outputs
output "amplify_app_id" {
  description = "The unique ID of the Amplify app"
  value       = module.amplify.app_id
}

output "amplify_app_arn" {
  description = "The Amazon Resource Name (ARN) of the Amplify app"
  value       = module.amplify.app_arn
}

output "amplify_app_name" {
  description = "The name of the Amplify app"
  value       = module.amplify.app_name
}

output "amplify_default_domain" {
  description = "The default domain for the Amplify app"
  value       = module.amplify.default_domain
}

output "amplify_app_url" {
  description = "The URL of the deployed Amplify app"
  value       = module.amplify.app_url
}

output "amplify_branch_name" {
  description = "The name of the Amplify branch"
  value       = module.amplify.branch_name
}

output "amplify_webhook_url" {
  description = "The webhook URL for the Amplify app"
  value       = module.amplify.webhook_url
}

# Additional outputs that might be useful for the root configuration
output "frontend_domain" {
  description = "The frontend domain for use in other configurations"
  value       = module.amplify.app_url
}

output "frontend_app_id" {
  description = "The frontend app ID for reference in other configurations"
  value       = module.amplify.app_id
}

# Parameter Store Integration Outputs
output "parameter_fetching_enabled" {
  description = "Whether parameter fetching from Parameter Store is enabled"
  value       = local.should_fetch_parameters
  sensitive = true
}

output "loaded_parameters" {
  description = "List of parameter names that were successfully loaded from Parameter Store"
  value       = keys(local.parameter_env_vars)
  sensitive = true
}

output "parameter_env_vars" {
  description = "Environment variables loaded from Parameter Store (sensitive values redacted)"
  value = {
    for k, v in local.parameter_env_vars :
    k => contains(["DATABASE_URL", "SECRET", "PASSWORD", "KEY"], upper(k)) ? "[REDACTED]" : v
  }
  sensitive = true
}

output "parameter_store_status" {
  description = "Status message about Parameter Store integration"
  value = local.should_fetch_parameters ? "Parameter Store fetching is ENABLED. Loaded ${length(local.parameter_env_vars)} parameters." : "Parameter Store fetching is DISABLED. Set 'fetch_parameters_from_store = true' to enable."
  sensitive = true
}

# Parameter Store Outputs for Frontend DNS/URL Storage
output "frontend_parameter_store_url" {
  description = "Parameter Store path for frontend URL"
  value       = aws_ssm_parameter.frontend_url.name
}

output "frontend_parameter_store_domain" {
  description = "Parameter Store path for frontend domain"
  value       = aws_ssm_parameter.frontend_domain.name
}

output "frontend_parameter_store_app_id" {
  description = "Parameter Store path for frontend app ID"
  value       = aws_ssm_parameter.frontend_app_id.name
}

output "stored_frontend_values" {
  description = "Summary of values stored in Parameter Store for other services"
  value = {
    frontend_url    = module.amplify.app_url
    frontend_domain = module.amplify.default_domain
    frontend_app_id = module.amplify.app_id
    parameter_paths = {
      url    = aws_ssm_parameter.frontend_url.name
      domain = aws_ssm_parameter.frontend_domain.name
      app_id = aws_ssm_parameter.frontend_app_id.name
    }
  }
} 
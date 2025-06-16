output "app_id" {
  description = "The unique ID of the Amplify app"
  value       = aws_amplify_app.main.id
}

output "app_arn" {
  description = "The Amazon Resource Name (ARN) of the Amplify app"
  value       = aws_amplify_app.main.arn
}

output "app_name" {
  description = "The name of the Amplify app"
  value       = aws_amplify_app.main.name
}

output "default_domain" {
  description = "The default domain for the Amplify app"
  value       = aws_amplify_app.main.default_domain
}

output "app_url" {
  description = "The URL of the deployed Amplify app"
  value       = "https://${aws_amplify_branch.main.branch_name}.${aws_amplify_app.main.default_domain}"
}

output "branch_name" {
  description = "The name of the deployed branch"
  value       = aws_amplify_branch.main.branch_name
}

output "webhook_url" {
  description = "The webhook URL for triggering builds"
  value       = aws_amplify_webhook.main.url
  sensitive   = true
}

output "repository_url" {
  description = "The connected repository URL"
  value       = var.github_repository_url
}

output "initial_deployment_triggered" {
  description = "Whether the initial deployment was triggered automatically"
  value       = "Initial deployment started automatically for ${aws_amplify_branch.main.branch_name} branch"
  depends_on  = [null_resource.trigger_initial_deployment]
} 
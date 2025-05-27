#####################################################
#   ECS SERVICE & TASK DEFINITION
#####################################################
output "ecs_service_name" {
  description = "Name of the ECS service"
  value       = aws_ecs_service.app_service.name
}

output "ecs_service_arn" {
  description = "ARN of the ECS service"
  value       = aws_ecs_service.app_service.id   # id == ARN for ecs_service
}

output "task_definition_arn" {
  description = "Full ARN of the active task definition"
  value       = aws_ecs_task_definition.app_task.arn
}

output "task_definition_family" {
  description = "Family name of the task definition"
  value       = aws_ecs_task_definition.app_task.family
}

output "task_definition_revision" {
  description = "Revision number of the task definition"
  value       = aws_ecs_task_definition.app_task.revision
}

#####################################################
#   LOGGING
#####################################################
output "cloudwatch_log_group_name" {
  description = "Name of the CloudWatch log group"
  value       = aws_cloudwatch_log_group.ecs_logs.name
}

output "cloudwatch_log_group_arn" {
  description = "ARN of the CloudWatch log group"
  value       = aws_cloudwatch_log_group.ecs_logs.arn
}

#####################################################
#   IAM ROLES
#####################################################
output "ecs_task_role_arn" {
  description = "ARN of the IAM role assumed by the task"
  value       = aws_iam_role.ecs_task_role.arn
}

output "ecs_execution_role_arn" {
  description = "ARN of the IAM role used for image-pull / logs"
  value       = aws_iam_role.ecs_execution_role.arn
}

#####################################################
#   SERVICE DISCOVERY (OPTIONAL)
#####################################################
output "service_discovery_namespace_id" {
  description = "ID of the Cloud Map namespace (if created)"
  value       = var.enable_service_discovery ? aws_service_discovery_private_dns_namespace.dns_ns[0].id : null
}

output "service_discovery_service_arn" {
  description = "ARN of the Cloud Map service (if created)"
  value       = var.enable_service_discovery ? aws_service_discovery_service.discovery_service[0].arn : null
}

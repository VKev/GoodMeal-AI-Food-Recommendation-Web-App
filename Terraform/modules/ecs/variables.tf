##############################
# REQUIRED – NO DEFAULTS
##############################
variable "project_name" {
  description = "Prefix for all ECS-related resources"
  type        = string
}

variable "aws_region" {
  description = "AWS region (used for log driver options)"
  type        = string
}

variable "container_name" {
  description = "Logical name of the container inside the task definition"
  type        = string
}

variable "ecr_repository_url" {
  description = "ECR repository URI, e.g. 123456789012.dkr.ecr.ap-southeast-1.amazonaws.com/myapp"
  type        = string
}

variable "image_tag" {
  description = "Image tag to deploy (latest, git-sha, …)"
  type        = string
}

variable "container_cpu" {
  description = "Hard CPU limit for the container (task-level CPU shares)"
  type        = number
}

variable "container_memory" {
  description = "Hard memory limit for the container (MiB)"
  type        = number
}

variable "container_port" {
  description = "Port your application listens on inside the container"
  type        = number
}

variable "environment_variables" {
  description = "List of environment variables, each as { name = \"...\", value = \"...\" }"
  type        = list(object({
    name  = string
    value = string
  }))
}

variable "health_check_command" {
  description = "CMD or CMD-SHELL array for container health checks"
  type        = list(string)
}

variable "ecs_cluster_id" {
  description = "Cluster ID, passed from the EC2 module"
  type        = string
}

variable "ecs_cluster_name" {
  description = "Cluster name, passed from the EC2 module (needed for auto-scaling resource_id)"
  type        = string
}

variable "vpc_id" {
  description = "VPC where service-discovery namespaces will be created"
  type        = string
}

##############################
# OPTIONAL – WITH DEFAULTS
##############################
variable "desired_count" {
  description = "Number of tasks to keep running"
  type        = number
  default     = 1
}

variable "enable_service_discovery" {
  description = "Create Cloud Map namespace + service?"
  type        = bool
  default     = false
}

variable "enable_auto_scaling" {
  description = "Create target-tracking scaling policies?"
  type        = bool
  default     = false
}

variable "target_group_arn" {
  description = "ALB / NLB target group ARN (blank to disable LB attachment)"
  type        = string
  default     = ""
}

variable "log_retention_days" {
  description = "Retention period for /ecs/<project> log group"
  type        = number
  default     = 30
}

variable "max_capacity" {
  description = "Upper limit for auto-scaling"
  type        = number
  default     = 4
}

variable "min_capacity" {
  description = "Lower limit for auto-scaling"
  type        = number
  default     = 1
}

variable "cpu_target_value" {
  description = "Target CPU utilisation percentage"
  type        = number
  default     = 50
}

variable "memory_target_value" {
  description = "Target memory utilisation percentage"
  type        = number
  default     = 70
}

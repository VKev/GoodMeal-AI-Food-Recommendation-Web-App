variable "project_name" {
  description = "Prefix for all ECS-related resources"
  type        = string
}

variable "aws_region" {
  description = "AWS region (used for log driver options)"
  type        = string
}

variable "vpc_id" {
  description = "VPC where service-discovery namespaces will be created"
  type        = string
}

variable "ecs_cluster_id" {
  description = "Cluster ID, passed from the EC2 module"
  type        = string
}

variable "ecs_cluster_name" {
  description = "Cluster name, passed from the EC2 module (needed for auto-scaling resource_id)"
  type        = string
}

variable "task_cpu" {
  description = "Total CPU units for the ECS task (e.g., 1024 for 1 vCPU)."
  type        = number
}

variable "task_memory" {
  description = "Total memory (in MiB) for the ECS task (e.g., 2048 for 2GB)."
  type        = number
}

variable "containers" {
  description = "A list of container definitions for the task."
  type = list(object({
    name                 = string
    image_repository_url = string
    image_tag            = string
    cpu                  = number # CPU units for this container
    memory               = number # Memory (MiB) for this container
    essential            = optional(bool, true)
    port_mappings = list(object({
      container_port = number
      host_port      = optional(number, 0) # 0 for dynamic host port assignment
      protocol       = optional(string, "tcp")
    }))
    environment_variables = optional(list(object({
      name  = string
      value = string
    })), [])
    health_check = optional(object({
      command     = list(string)
      interval    = optional(number, 30)
      timeout     = optional(number, 5)
      retries     = optional(number, 3)
      startPeriod = optional(number, 60) # Time to ignore health check on startup
    }))
    enable_service_discovery = optional(bool, false)
    service_discovery_port   = optional(number) # The containerPort to register with service discovery
  }))
  default = []
}

variable "target_groups" {
  description = "A list of load balancer target group configurations. Each object links a target group to a container and port."
  type = list(object({
    target_group_arn = string
    container_name   = string # Name of the container (must match one in 'containers' variable)
    container_port   = number # Port of the container to link with the target group
  }))
  default = []
}

variable "desired_count" {
  description = "Number of tasks to keep running."
  type        = number
  default     = 1
}

variable "enable_service_discovery" {
  description = "Globally enable/disable Cloud Map namespace creation. Individual containers also need 'enable_service_discovery = true'."
  type        = bool
  default     = false
}

variable "enable_auto_scaling" {
  description = "Create target-tracking scaling policies for the service?"
  type        = bool
  default     = false
}

variable "log_retention_days" {
  description = "Retention period for /ecs/<project> log group in CloudWatch."
  type        = number
  default     = 30
}

variable "max_capacity" {
  description = "Upper limit for auto-scaling (number of tasks)."
  type        = number
  default     = 4
}

variable "min_capacity" {
  description = "Lower limit for auto-scaling (number of tasks)."
  type        = number
  default     = 1
}

variable "cpu_target_value" {
  description = "Target CPU utilisation percentage for auto-scaling."
  type        = number
  default     = 50
}

variable "memory_target_value" {
  description = "Target memory utilisation percentage for auto-scaling."
  type        = number
  default     = 70
}
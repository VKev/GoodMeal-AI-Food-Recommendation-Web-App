variable "project_name" {
  description = "A prefix used for naming resources to ensure uniqueness and grouping."
  type        = string
}

variable "vpc_id" {
  description = "The ID of the VPC where the ALB and related resources will be deployed."
  type        = string
}

variable "public_subnet_ids" {
  description = "A list of public subnet IDs where the Application Load Balancer will be placed."
  type        = list(string)
}

variable "target_groups_definition" {
  description = "A list of configurations for Application Load Balancer target groups."
  type = list(object({
    name_suffix = string # Unique suffix for the target group name (e.g., "app1-frontend", "api-service")
    port        = number # The port on which targets receive traffic
    protocol    = optional(string, "HTTP") # Protocol for routing traffic to targets (HTTP, HTTPS)
    target_type = optional(string, "instance") # Target type: "instance", "ip", or "lambda"

    health_check = optional(object({
      enabled             = optional(bool, true)    # Whether health checks are enabled
      path                = optional(string, "/")   # Destination for health checks
      port                = optional(string, "traffic-port") # Port for health checks
      protocol            = optional(string, "HTTP")  # Protocol for health checks (HTTP, HTTPS, TCP)
      healthy_threshold   = optional(number, 3)     # Number of consecutive successful checks for healthy status
      unhealthy_threshold = optional(number, 2)     # Number of consecutive failed checks for unhealthy status
      interval            = optional(number, 30)    # Approximate time (seconds) between health checks
      timeout             = optional(number, 5)     # Amount of time (seconds) to wait for a response before failing
      matcher             = optional(string, "200") # Expected HTTP codes for success (e.g., "200", "200-299")
    }), {}) # Defaults to an empty object, implying provider defaults for unspecified fields if health_check block is rendered.
  }))
  default = []
}

variable "default_listener_action" {
  description = "Configuration for the default action of the HTTP listener on port 80."
  type = object({
    type = string # Action type: "forward" or "fixed-response"
    target_group_suffix = optional(string) # Required if type is "forward". Must match a 'name_suffix' in 'target_groups_definition'.
    fixed_response = optional(object({     # Required if type is "fixed-response"
      content_type = string                # E.g., "text/plain", "application/json"
      message_body = string                # Content of the fixed response
      status_code  = string                # HTTP status code (e.g., "404", "200")
    }))
  })
  # Example for forwarding: { type = "forward", target_group_suffix = "default-app" }
  # Example for fixed response: { type = "fixed-response", fixed_response = { content_type = "text/plain", message_body = "Resource Not Found", status_code = "404" } }
}

variable "listener_rules_definition" {
  description = "A list of listener rule configurations for the HTTP listener."
  type = list(object({
    priority            = number # Rule priority (1-50000, lower numbers evaluated first, must be unique per listener)
    target_group_suffix = string # 'name_suffix' of a target group defined in 'target_groups_definition'.
    conditions = list(object({   # List of conditions that must ALL be met for the rule to apply
      path_pattern = optional(object({
        values = list(string) # e.g., ["/app1/*", "/service-a/*"]
      }))
      host_header = optional(object({
        values = list(string) # e.g., ["app1.example.com", "*.api.example.com"]
      }))
      http_request_method = optional(object({
        values = list(string) # e.g., ["GET", "POST", "PUT"]
      }))
      # Future conditions like source_ip, http_header (for specific headers), query_string can be added here.
    }))
  }))
  default = []
}
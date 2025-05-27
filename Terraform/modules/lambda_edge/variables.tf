variable "function_name" {
  description = "Name of the Lambda function"
  type        = string
  default     = "TerraformSetCookieEdgeFunction"
}

variable "runtime" {
  description = "Lambda runtime"
  type        = string
  default     = "nodejs22.x"
}

variable "handler" {
  description = "Lambda handler"
  type        = string
  default     = "index.handler"
}

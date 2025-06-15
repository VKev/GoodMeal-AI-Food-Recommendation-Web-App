variable "app_name" {
  description = "Name of the Amplify application"
  type        = string
}

variable "github_repository_url" {
  description = "GitHub repository URL for the Next.js application"
  type        = string
}

variable "github_access_token" {
  description = "GitHub personal access token for repository access"
  type        = string
  sensitive   = true
}

variable "branch_name" {
  description = "Branch name to deploy from"
  type        = string
  default     = "main"
}

variable "environment_variables" {
  description = "Environment variables for the Amplify app"
  type        = map(string)
  default     = {}
}

variable "build_spec" {
  description = "Build specification for the Next.js application"
  type        = string
  default     = ""
}

variable "enable_auto_branch_creation" {
  description = "Enable automatic branch creation and deployment"
  type        = bool
  default     = false
}

variable "enable_branch_auto_build" {
  description = "Enable automatic build on branch push"
  type        = bool
  default     = true
}

variable "enable_branch_auto_deletion" {
  description = "Enable automatic branch deletion"
  type        = bool
  default     = false
}

variable "framework" {
  description = "Framework for the application (Next.js - SSR for server-side rendering, Next.js - SSG for static generation)"
  type        = string
  default     = "Next.js - SSR"

  validation {
    condition = contains([
      "Next.js - SSR",
      "Next.js - SSG"
    ], var.framework)
    error_message = "Framework must be either 'Next.js - SSR' or 'Next.js - SSG'."
  }
}

variable "rendering_mode" {
  description = "Rendering mode for Next.js app (SSR or SSG)"
  type        = string
  default     = "SSR"

  validation {
    condition     = contains(["SSR", "SSG"], var.rendering_mode)
    error_message = "Rendering mode must be either 'SSR' or 'SSG'."
  }
}

variable "tags" {
  description = "Tags to apply to the Amplify resources"
  type        = map(string)
  default     = {}
}

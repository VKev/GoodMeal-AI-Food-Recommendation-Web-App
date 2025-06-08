variable "origin_domain_name" {
  description = "The origin domain name for CloudFront"
  type        = string
}

variable "origin_path" {
  description = "The origin path to append to the domain name"
  type        = string
}

variable "set_cookie_lambda_arn" {
  description = "The qualified ARN of the Lambda@Edge function for /set-cookie behavior"
  type        = string
}

variable "bucket_secret_referer" {
  description = "Custom header referer for the bucket"
  type = string
}

variable "project_name" {
  description = "The name of the project"
  type        = string
}

variable "project_name" {
  description = "Name of the project for resource naming"
  type        = string
  default     = "goodmeal"
}

variable "vpc_id" {
  description = "ID of the VPC where resources will be created"
  type        = string
}

variable "vpc_cidr" {
  description = "CIDR block of the VPC"
  type        = string
}

variable "subnet_id" {
  description = "ID of the subnet where EC2 instance will be launched"
  type        = string
}

variable "instance_type" {
  description = "EC2 instance type (free tier eligible: t2.micro)"
  type        = string
  default     = "t2.micro"
}

variable "root_volume_size" {
  description = "Size of the root EBS volume in GB (free tier eligible: up to 30GB)"
  type        = number
  default     = 30
}

# Key pair variables removed - using local public_key.pem file

variable "associate_public_ip" {
  description = "Whether to associate an Elastic IP with the EC2 instance"
  type        = bool
  default     = true
}

variable "environment" {
  description = "Environment name (e.g., dev, staging, prod)"
  type        = string
  default     = "dev"
} 
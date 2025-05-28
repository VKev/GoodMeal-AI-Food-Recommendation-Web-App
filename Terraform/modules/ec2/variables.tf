##############################
# REQUIRED
##############################
variable "project_name" {
  description = "Prefix for all EC2 / cluster resources"
  type        = string
}

variable "vpc_id" {
  description = "VPC where the container instance will live"
  type        = string
}

variable "vpc_cidr" {
  description = "CIDR of the VPC (used to allow dynamic ECS ports)"
  type        = string
}

variable "subnet_id" {
  description = "Subnet ID (public or private-with-NAT) for the EC2 instance"
  type        = string
}

##############################
# OPTIONAL
##############################
variable "instance_type" {
  description = "EC2 instance type for the container instance"
  type        = string
  default     = "t3.small"
}

variable "root_volume_size" {
  description = "Root EBS volume size in GiB"
  type        = number
  default     = 30
}

variable "associate_public_ip" {
  description = "Attach an Elastic IP for direct SSH access?"
  type        = bool
  default     = true
}

variable "alb_security_group_id" {
  description = "Security-group ID of the Application Load Balancer"
  type        = string
}

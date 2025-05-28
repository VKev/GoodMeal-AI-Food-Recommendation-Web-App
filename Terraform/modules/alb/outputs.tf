output "alb_dns_name" {
  value       = aws_lb.this.dns_name
  description = "The public DNS name of the Application Load Balancer."
}

output "alb_arn" {
  value       = aws_lb.this.arn
  description = "The ARN of the Application Load Balancer."
}

output "alb_zone_id" {
  value       = aws_lb.this.zone_id
  description = "The canonical hosted zone ID of the Application Load Balancer (to be used in a Route 53 Alias record)."
}

output "alb_sg_id" {
  value       = aws_security_group.alb_sg.id
  description = "The ID of the security group attached to the Application Load Balancer."
}

output "http_listener_arn" {
  value       = aws_lb_listener.http.arn
  description = "The ARN of the HTTP listener on port 80."
}

# If you create an HTTPS listener, you would add its ARN here too.
# output "https_listener_arn" {
#   value       = aws_lb_listener.https.arn # Assuming you name it 'https'
#   description = "The ARN of the HTTPS listener on port 443."
# }

output "target_group_arns_map" {
  value = {
    for suffix, tg in aws_lb_target_group.this : suffix => tg.arn
  }
  description = "A map of target group ARNs, keyed by their 'name_suffix'. This is useful for passing to ECS service configurations."
}

output "target_group_names_map" {
  value = {
    for suffix, tg in aws_lb_target_group.this : suffix => tg.name
  }
  description = "A map of target group names, keyed by their 'name_suffix'."
}
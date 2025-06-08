resource "aws_security_group" "alb_sg" {
  name_prefix = "${var.project_name}-alb-sg-" # Added trailing hyphen for better readability if name_prefix truncates
  vpc_id      = var.vpc_id
  description = "ALB Security Group allowing public HTTP/HTTPS access"

  ingress {
    description = "Allow HTTP traffic from anywhere"
    from_port   = 80
    to_port     = 80
    protocol    = "tcp"
    cidr_blocks = ["0.0.0.0/0"]
  }

  ingress {
    description = "Allow HTTPS traffic from anywhere"
    from_port   = 443
    to_port     = 443
    protocol    = "tcp"
    cidr_blocks = ["0.0.0.0/0"]
  }

  egress {
    description = "Allow all outbound traffic"
    from_port   = 0
    to_port     = 0
    protocol    = "-1" # Represents all protocols
    cidr_blocks = ["0.0.0.0/0"]
  }

  tags = {
    Name = "${var.project_name}-alb-sg"
  }
}

resource "aws_lb" "this" {
  name               = "${var.project_name}-alb"
  internal           = false # Assuming public-facing, change if internal
  load_balancer_type = "application"
  security_groups    = [aws_security_group.alb_sg.id]
  subnets            = var.public_subnet_ids

  enable_deletion_protection = false # Consider setting to true for production

  tags = {
    Name = "${var.project_name}-alb"
  }
}

resource "aws_lb_target_group" "this" {
  for_each = { for tg_conf in var.target_groups_definition : tg_conf.name_suffix => tg_conf }

  name        = "${var.project_name}-tg-${each.key}" # each.key is tg_conf.name_suffix
  port        = each.value.port
  protocol    = each.value.protocol
  target_type = each.value.target_type
  vpc_id      = var.vpc_id

  health_check {
    enabled             = each.value.health_check.enabled
    path                = each.value.health_check.path
    port                = each.value.health_check.port
    protocol            = each.value.health_check.protocol
    healthy_threshold   = each.value.health_check.healthy_threshold
    unhealthy_threshold = each.value.health_check.unhealthy_threshold
    interval            = each.value.health_check.interval
    timeout             = each.value.health_check.timeout
    matcher             = each.value.health_check.matcher
  }

  tags = {
    Name    = "${var.project_name}-tg-${each.key}"
    Project = var.project_name
  }

  lifecycle {
    create_before_destroy = true # Useful for updates without downtime if names don't change drastically
  }
}

resource "aws_lb_listener" "http" {
  load_balancer_arn = aws_lb.this.arn
  port              = 80
  protocol          = "HTTP"

  default_action {
    type = var.default_listener_action.type
    target_group_arn = var.default_listener_action.type == "forward" && var.default_listener_action.target_group_suffix != null ? (
      aws_lb_target_group.this[var.default_listener_action.target_group_suffix].arn
      ) : null

    dynamic "fixed_response" {
      for_each = var.default_listener_action.type == "fixed-response" && var.default_listener_action.fixed_response != null ? [var.default_listener_action.fixed_response] : []
      content {
        content_type = fixed_response.value.content_type
        message_body = fixed_response.value.message_body
        status_code  = fixed_response.value.status_code
      }
    }
  }

  tags = {
    Name = "${var.project_name}-alb-http-listener"
  }
}

resource "aws_lb_listener_rule" "rules" {
  for_each = { for idx, rule_conf in var.listener_rules_definition : idx => rule_conf } # Use index for unique keying

  listener_arn = aws_lb_listener.http.arn
  priority     = each.value.priority

  action {
    type             = "forward"
    target_group_arn = aws_lb_target_group.this[each.value.target_group_suffix].arn
  }

  dynamic "condition" {
    for_each = each.value.conditions
    content {
      dynamic "path_pattern" {
        for_each = condition.value.path_pattern != null ? [condition.value.path_pattern] : []
        content {
          values = path_pattern.value.values
        }
      }
      dynamic "host_header" {
        for_each = condition.value.host_header != null ? [condition.value.host_header] : []
        content {
          values = host_header.value.values
        }
      }
      dynamic "http_request_method" {
        for_each = condition.value.http_request_method != null ? [condition.value.http_request_method] : []
        content {
          values = http_request_method.value.values
        }
      }
      # Add more dynamic condition blocks here if you expand the variable definition
    }
  }

  tags = {
    Name     = "${var.project_name}-listener-rule-${each.value.priority}-${each.value.target_group_suffix}"
    Priority = each.value.priority
  }
}
##########################################################################
# REQUIRED VARIABLES (declare in ecs/variables.tf or from the root module)
##########################################################################
# var.project_name             – Prefix for names/tags
# var.aws_region               – e.g. "ap-southeast-1"
# var.container_name           – logical name inside the task def
# var.ecr_repository_url       – 111122223333.dkr.ecr.ap-southeast-1.amazonaws.com/myapp
# var.image_tag                – e.g. "latest"
# var.container_cpu            – 256  (hard limit, unit = CPU shares)
# var.container_memory         – 512  (hard limit, MiB)
# var.container_port           – 8080
# var.environment_variables    – list of { name = "...", value = "..." }
# var.health_check_command     – ["CMD-SHELL", "curl -f http://localhost:8080/health || exit 1"]
# var.ecs_cluster_id           – aws_ecs_cluster.main.id  (passed from ec2 module)
# var.ecs_cluster_name         – aws_ecs_cluster.main.name
# var.vpc_id                   – VPC where service discovery lives
# --- OPTIONAL ---
# var.enable_service_discovery – default false
# var.enable_auto_scaling      – default false
# var.desired_count            – default 1
# var.target_group_arn         – ""  (if ALB target group supplied)
# var.log_retention_days       – default 30
# var.max_capacity             – default 4
# var.min_capacity             – default 1
# var.cpu_target_value         – default 50
# var.memory_target_value      – default 70
##########################################################################

#################################
# CloudWatch log group (one-off)
#################################
resource "aws_cloudwatch_log_group" "ecs_logs" {
  name              = "/ecs/${var.project_name}"
  retention_in_days = var.log_retention_days
  tags              = { Name = "${var.project_name}-ecs-logs" }
}

####################
# IAM FOR THE TASK
####################
resource "aws_iam_role" "ecs_task_role" {
  name = "${var.project_name}-ecs-task-role"

  assume_role_policy = jsonencode({
    Version = "2012-10-17"
    Statement = [{
      Action = "sts:AssumeRole"
      Effect = "Allow"
      Principal = { Service = "ecs-tasks.amazonaws.com" }
    }]
  })
}

resource "aws_iam_role" "ecs_execution_role" {
  name = "${var.project_name}-ecs-execution-role"

  assume_role_policy = jsonencode({
    Version = "2012-10-17"
    Statement = [{
      Action = "sts:AssumeRole"
      Effect = "Allow"
      Principal = { Service = "ecs-tasks.amazonaws.com" }
    }]
  })
}

# Policy: the task itself only needs to write its own log streams
resource "aws_iam_role_policy" "ecs_task_policy" {
  name = "${var.project_name}-ecs-task-policy"
  role = aws_iam_role.ecs_task_role.id

  policy = jsonencode({
    Version = "2012-10-17"
    Statement = [{
      Effect   = "Allow"
      Action   = ["logs:CreateLogStream", "logs:PutLogEvents"]
      Resource = "${aws_cloudwatch_log_group.ecs_logs.arn}:*"
    }]
  })
}

# Pre-defined AWS-managed policies
resource "aws_iam_role_policy_attachment" "ecs_execution_managed" {
  role       = aws_iam_role.ecs_execution_role.name
  policy_arn = "arn:aws:iam::aws:policy/service-role/AmazonECSTaskExecutionRolePolicy"
}

resource "aws_iam_role_policy_attachment" "ecs_task_ecr_pull" {
  role       = aws_iam_role.ecs_task_role.name
  policy_arn = "arn:aws:iam::aws:policy/AmazonEC2ContainerRegistryReadOnly"
}

##############################
# TASK DEFINITION
##############################
resource "aws_ecs_task_definition" "app_task" {
  family                   = "${var.project_name}-task"
  network_mode             = "bridge"
  requires_compatibilities = ["EC2"]
  cpu                      = tostring(var.container_cpu)
  memory                   = tostring(var.container_memory)

  task_role_arn      = aws_iam_role.ecs_task_role.arn
  execution_role_arn = aws_iam_role.ecs_execution_role.arn

  container_definitions = jsonencode([
    {
      name      = var.container_name
      image     = "${var.ecr_repository_url}:${var.image_tag}"
      cpu       = var.container_cpu
      memory    = var.container_memory
      essential = true

      portMappings = [{
        containerPort = var.container_port
        hostPort      = 0            # dynamic host port
        protocol      = "tcp"
      }]

      environment = var.environment_variables

      logConfiguration = {
        logDriver = "awslogs"
        options = {
          awslogs-group         = aws_cloudwatch_log_group.ecs_logs.name
          awslogs-region        = var.aws_region
          awslogs-stream-prefix = "ecs"
        }
      }

      healthCheck = {
        command     = var.health_check_command
        interval    = 30
        timeout     = 5
        retries     = 3
        startPeriod = 60
      }
    }
  ])

  tags = { Name = "${var.project_name}-task-definition" }
}

#################
# SERVICE
#################
resource "aws_ecs_service" "app_service" {
  name            = "${var.project_name}-service"
  cluster         = var.ecs_cluster_id
  task_definition = aws_ecs_task_definition.app_task.arn
  desired_count   = var.desired_count
  launch_type     = "EC2"

  ordered_placement_strategy {
    type  = "spread"
    field = "instanceId"
  }

  ###################################################
  # OPTIONAL: Service discovery & ALB integration
  ###################################################
  dynamic "service_registries" {
    for_each = var.enable_service_discovery ? [1] : []
    content {
      registry_arn = aws_service_discovery_service.discovery_service[0].arn
    }
  }

  dynamic "load_balancer" {
    for_each = var.target_group_arn != "" ? [1] : []
    content {
      target_group_arn = var.target_group_arn
      container_name   = var.container_name
      container_port   = var.container_port
    }
  }

  ###################################################
  deployment_maximum_percent         = 200
  deployment_minimum_healthy_percent = 50

  lifecycle { ignore_changes = [desired_count] }

  tags = { Name = "${var.project_name}-ecs-service" }

  depends_on = [
    aws_iam_role_policy_attachment.ecs_task_ecr_pull,
    aws_iam_role_policy_attachment.ecs_execution_managed
  ]
}

#######################################
# OPTIONAL PRIVATE SERVICE DISCOVERY
#######################################
resource "aws_service_discovery_private_dns_namespace" "dns_ns" {
  count       = var.enable_service_discovery ? 1 : 0
  name        = "${var.project_name}.local"
  vpc         = var.vpc_id
  description = "Service discovery namespace"
}

resource "aws_service_discovery_service" "discovery_service" {
  count = var.enable_service_discovery ? 1 : 0
  name  = var.container_name

  dns_config {
    namespace_id = aws_service_discovery_private_dns_namespace.dns_ns[0].id
    routing_policy = "MULTIVALUE"

    dns_records {
      ttl  = 10
      type = "A"
    }
  }
}

###################################################
# OPTIONAL TARGET-TRACKING AUTO-SCALING
###################################################
resource "aws_appautoscaling_target" "ecs_target" {
  count              = var.enable_auto_scaling ? 1 : 0
  max_capacity       = var.max_capacity
  min_capacity       = var.min_capacity
  resource_id        = "service/${var.ecs_cluster_name}/${aws_ecs_service.app_service.name}"
  scalable_dimension = "ecs:service:DesiredCount"
  service_namespace  = "ecs"
}

resource "aws_appautoscaling_policy" "ecs_cpu_policy" {
  count              = var.enable_auto_scaling ? 1 : 0
  name               = "${var.project_name}-cpu-scaling"
  policy_type        = "TargetTrackingScaling"
  resource_id        = aws_appautoscaling_target.ecs_target[0].resource_id
  scalable_dimension = aws_appautoscaling_target.ecs_target[0].scalable_dimension
  service_namespace  = aws_appautoscaling_target.ecs_target[0].service_namespace

  target_tracking_scaling_policy_configuration {
    predefined_metric_specification {
      predefined_metric_type = "ECSServiceAverageCPUUtilization"
    }
    target_value = var.cpu_target_value
  }
}

resource "aws_appautoscaling_policy" "ecs_memory_policy" {
  count              = var.enable_auto_scaling ? 1 : 0
  name               = "${var.project_name}-memory-scaling"
  policy_type        = "TargetTrackingScaling"
  resource_id        = aws_appautoscaling_target.ecs_target[0].resource_id
  scalable_dimension = aws_appautoscaling_target.ecs_target[0].scalable_dimension
  service_namespace  = aws_appautoscaling_target.ecs_target[0].service_namespace

  target_tracking_scaling_policy_configuration {
    predefined_metric_specification {
      predefined_metric_type = "ECSServiceAverageMemoryUtilization"
    }
    target_value = var.memory_target_value
  }
}

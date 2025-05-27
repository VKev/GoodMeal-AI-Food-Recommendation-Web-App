# ECS Module

This Terraform module creates an ECS service that runs containers from your ECR repository on the EC2 instance created by the EC2 module.

## Features

- **ECS Task Definition**: Defines how your container should run
- **ECS Service**: Manages the desired number of running tasks
- **IAM Roles**: Proper permissions for task execution and application access
- **CloudWatch Logging**: Centralized logging for your containers
- **Auto Scaling**: Optional auto scaling based on CPU/Memory utilization
- **Service Discovery**: Optional service discovery for microservices
- **Load Balancer Integration**: Optional integration with Application Load Balancer

## Usage

```hcl
module "ecs" {
  source = "./modules/ecs"
  
  project_name       = "goodmeal"
  aws_region        = "us-east-1"
  vpc_id            = module.vpc.vpc_id
  ecs_cluster_id    = module.ec2.ecs_cluster_arn
  ecs_cluster_name  = module.ec2.ecs_cluster_name
  
  # Container configuration
  ecr_repository_url = "123456789012.dkr.ecr.us-east-1.amazonaws.com/goodmeal-app"
  image_tag         = "latest"
  container_name    = "goodmeal-app"
  container_port    = 80
  container_memory  = 512
  container_cpu     = 256
  desired_count     = 1
  
  # Environment variables
  environment_variables = [
    {
      name  = "NODE_ENV"
      value = "production"
    },
    {
      name  = "PORT"
      value = "80"
    }
  ]
  
  # Optional features
  enable_auto_scaling      = false
  enable_service_discovery = false
}
```

## Prerequisites

1. **ECR Repository**: You must have an ECR repository with your container image
2. **EC2 Module**: This module depends on the EC2 module which creates the ECS cluster
3. **VPC Module**: A VPC must exist for networking

## Container Requirements

Your container image should:
- Expose the port specified in `container_port`
- Handle graceful shutdowns (SIGTERM signals)
- Include a health check endpoint if using custom health checks
- Be optimized for the allocated CPU and memory resources

## Environment Variables

You can pass environment variables to your container:

```hcl
environment_variables = [
  {
    name  = "DATABASE_URL"
    value = "postgresql://user:pass@host:5432/db"
  },
  {
    name  = "API_KEY"
    value = "your-api-key"
  }
]
```

## Auto Scaling

Enable auto scaling to automatically adjust the number of running tasks:

```hcl
enable_auto_scaling = true
min_capacity       = 1
max_capacity       = 10
cpu_target_value   = 70
memory_target_value = 80
```

## Service Discovery

Enable service discovery for microservices communication:

```hcl
enable_service_discovery = true
```

This creates a private DNS namespace `{project_name}.local` where your service can be reached at `{container_name}.{project_name}.local`.

## Load Balancer Integration

To integrate with an Application Load Balancer, provide the target group ARN:

```hcl
target_group_arn = "arn:aws:elasticloadbalancing:region:account:targetgroup/name/id"
```

## Monitoring

The module automatically creates:
- CloudWatch log group for container logs
- ECS service metrics in CloudWatch
- Auto scaling metrics (if enabled)

## Security

The module creates IAM roles with minimal required permissions:
- **Task Execution Role**: Pulls images from ECR and writes logs to CloudWatch
- **Task Role**: Runtime permissions for your application (customize as needed)

## Outputs

- `ecs_service_name`: Name of the ECS service
- `ecs_service_arn`: ARN of the ECS service
- `task_definition_arn`: ARN of the task definition
- `cloudwatch_log_group_name`: Name of the CloudWatch log group

## Cost Optimization

For cost optimization:
- Use `t2.micro` instance type (free tier eligible)
- Set appropriate CPU and memory limits
- Use log retention to control CloudWatch costs
- Monitor auto scaling to prevent unexpected scaling

## Troubleshooting

1. **Service not starting**: Check CloudWatch logs for container errors
2. **Health check failures**: Verify your application responds on the health check endpoint
3. **Resource constraints**: Increase CPU/memory allocation if tasks are being killed
4. **Network issues**: Verify security group rules allow traffic on the container port 
# GoodMeal ECS Deployment Guide

This guide walks you through deploying your containerized application using ECS on EC2 infrastructure.

## Prerequisites

1. **AWS CLI configured** with appropriate permissions
2. **Terraform installed** (version 1.0+)
3. **Docker image pushed to ECR** repository
4. **SSH key pair** for EC2 access

## Step 1: Prepare Your ECR Repository

First, create an ECR repository and push your Docker image:

```bash
# Create ECR repository
aws ecr create-repository --repository-name goodmeal-app --region us-east-1

# Get login token
aws ecr get-login-password --region us-east-1 | docker login --username AWS --password-stdin 123456789012.dkr.ecr.us-east-1.amazonaws.com

# Tag your image
docker tag goodmeal-app:latest 123456789012.dkr.ecr.us-east-1.amazonaws.com/goodmeal-app:latest

# Push to ECR
docker push 123456789012.dkr.ecr.us-east-1.amazonaws.com/goodmeal-app:latest
```

## Step 2: Configure Terraform Variables

1. Copy the example variables file:
```bash
cp terraform.tfvars.example terraform.tfvars
```

2. Edit `terraform.tfvars` with your specific values:
```hcl
# Project Configuration
project_name = "goodmeal"
aws_region   = "us-east-1"

# ECS Configuration
ecr_repository_url = "123456789012.dkr.ecr.us-east-1.amazonaws.com/goodmeal-app"  # Your actual ECR URL
image_tag         = "latest"
container_name    = "goodmeal-app"
container_port    = 3000  # Your app's port
container_memory  = 512
container_cpu     = 256
desired_count     = 1

# Environment Variables for your application
environment_variables = [
  {
    name  = "NODE_ENV"
    value = "production"
  },
  {
    name  = "PORT"
    value = "3000"
  },
  {
    name  = "DATABASE_URL"
    value = "your-database-connection-string"
  }
]
```

## Step 3: Prepare SSH Key

Create an SSH key pair for EC2 access:

```bash
# Generate SSH key pair
ssh-keygen -t rsa -b 2048 -f ~/.ssh/goodmeal-key

# Copy public key to the EC2 module directory
cp ~/.ssh/goodmeal-key.pub Terraform/modules/ec2/public_key.pub
```

## Step 4: Deploy Infrastructure

1. Initialize Terraform:
```bash
cd Terraform
terraform init
```

2. Plan the deployment:
```bash
terraform plan
```

3. Apply the configuration:
```bash
terraform apply
```

4. Note the outputs, especially:
   - `ec2_public_ip`: IP address of your EC2 instance
   - `ecs_cluster_name`: Name of your ECS cluster
   - `ecs_service_name`: Name of your ECS service

## Step 5: Verify Deployment

1. **Check ECS Service Status**:
```bash
aws ecs describe-services --cluster goodmeal-cluster --services goodmeal-service --region us-east-1
```

2. **Check Running Tasks**:
```bash
aws ecs list-tasks --cluster goodmeal-cluster --region us-east-1
```

3. **View Container Logs**:
```bash
aws logs tail /ecs/goodmeal --follow --region us-east-1
```

4. **SSH to EC2 Instance** (if needed):
```bash
ssh -i ~/.ssh/goodmeal-key ec2-user@<EC2_PUBLIC_IP>
```

## Step 6: Access Your Application

Your application will be accessible through the EC2 instance's public IP on a dynamic port. To find the port:

1. **Get the task ARN**:
```bash
aws ecs list-tasks --cluster goodmeal-cluster --region us-east-1
```

2. **Describe the task to get port mapping**:
```bash
aws ecs describe-tasks --cluster goodmeal-cluster --tasks <TASK_ARN> --region us-east-1
```

3. **Access your application**:
```
http://<EC2_PUBLIC_IP>:<DYNAMIC_PORT>
```

## Step 7: Update Your Application

To deploy a new version of your application:

1. **Build and push new image**:
```bash
docker build -t goodmeal-app:v2 .
docker tag goodmeal-app:v2 123456789012.dkr.ecr.us-east-1.amazonaws.com/goodmeal-app:v2
docker push 123456789012.dkr.ecr.us-east-1.amazonaws.com/goodmeal-app:v2
```

2. **Update terraform.tfvars**:
```hcl
image_tag = "v2"
```

3. **Apply changes**:
```bash
terraform apply
```

## Monitoring and Troubleshooting

### CloudWatch Logs
View your application logs:
```bash
aws logs describe-log-streams --log-group-name /ecs/goodmeal --region us-east-1
aws logs get-log-events --log-group-name /ecs/goodmeal --log-stream-name <STREAM_NAME> --region us-east-1
```

### ECS Service Events
Check service events for deployment issues:
```bash
aws ecs describe-services --cluster goodmeal-cluster --services goodmeal-service --region us-east-1 --query 'services[0].events'
```

### Common Issues

1. **Task keeps stopping**:
   - Check CloudWatch logs for application errors
   - Verify memory/CPU allocation is sufficient
   - Ensure health check endpoint is working

2. **Can't pull image**:
   - Verify ECR repository URL is correct
   - Check IAM permissions for ECS task execution role
   - Ensure image exists in ECR

3. **Service not starting**:
   - Check ECS cluster has available capacity
   - Verify security group allows required ports
   - Check task definition is valid

## Scaling Your Application

### Manual Scaling
Update the desired count in terraform.tfvars:
```hcl
desired_count = 3
```

### Auto Scaling
Enable auto scaling:
```hcl
enable_auto_scaling = true
min_capacity       = 1
max_capacity       = 10
cpu_target_value   = 70
```

## Security Best Practices

1. **Restrict SSH access** in the security group to your IP only
2. **Use secrets management** for sensitive environment variables
3. **Enable VPC Flow Logs** for network monitoring
4. **Regularly update** your container images
5. **Use least privilege** IAM policies

## Cost Optimization

1. **Use t2.micro** for development (free tier eligible)
2. **Set appropriate log retention** to control CloudWatch costs
3. **Monitor auto scaling** to prevent unexpected scaling
4. **Use spot instances** for non-critical workloads (requires additional configuration)

## Cleanup

To destroy all resources:
```bash
terraform destroy
```

**Warning**: This will delete all resources including data. Make sure to backup any important data first.

## Next Steps

1. **Set up Application Load Balancer** for better traffic distribution
2. **Implement CI/CD pipeline** for automated deployments
3. **Add RDS database** for persistent data storage
4. **Configure CloudFront** for content delivery
5. **Set up monitoring and alerting** with CloudWatch alarms 
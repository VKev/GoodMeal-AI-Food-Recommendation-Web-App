#!/bin/bash

# Update the system
yum update -y

# Install essential packages
yum install -y docker awscli

# Start and enable Docker service
systemctl start docker
systemctl enable docker

# Add ec2-user to docker group
usermod -a -G docker ec2-user

# Configure ECS agent
echo "ECS_CLUSTER=${cluster_name}" >> /etc/ecs/ecs.config
echo "ECS_ENABLE_CONTAINER_METADATA=true" >> /etc/ecs/ecs.config
echo "ECS_ENABLE_TASK_IAM_ROLE=true" >> /etc/ecs/ecs.config
echo "ECS_ENABLE_TASK_IAM_ROLE_NETWORK_HOST=true" >> /etc/ecs/ecs.config

# Start and enable ECS agent
systemctl start ecs
systemctl enable ecs

# Install CloudWatch agent (optional for monitoring)
yum install -y amazon-cloudwatch-agent

# Create a simple health check script
cat > /home/ec2-user/health-check.sh << 'EOF'
#!/bin/bash
# Simple health check script
echo "Instance is healthy - $(date)" > /tmp/health-status
curl -s http://169.254.169.254/latest/meta-data/instance-id > /tmp/instance-id
EOF

chmod +x /home/ec2-user/health-check.sh
chown ec2-user:ec2-user /home/ec2-user/health-check.sh

# Run health check on startup
/home/ec2-user/health-check.sh

# Log successful completion
echo "ECS optimized instance setup completed successfully" >> /var/log/user-data.log 
# VPC Module

This module creates a complete VPC infrastructure with public and private subnets for the GoodMeal AI Food Recommendation System.

## Resources Created

- **VPC**: Main virtual private cloud with DNS support
- **Public Subnet**: Subnet with direct internet access via Internet Gateway
- **Private Subnet**: Subnet with internet access via NAT Gateway
- **Internet Gateway**: Provides internet access to public subnet
- **NAT Gateway**: Provides outbound internet access to private subnet
- **Elastic IP**: Static IP for NAT Gateway
- **Route Tables**: Separate routing for public and private subnets
- **Route Table Associations**: Links subnets to their respective route tables

## Architecture

```
Internet
    |
Internet Gateway
    |
Public Subnet (10.0.1.0/24)
    |
NAT Gateway
    |
Private Subnet (10.0.2.0/24)
```

## Variables

| Name | Description | Type | Default |
|------|-------------|------|---------|
| project_name | Name of the project for resource naming | string | "goodmeal" |
| vpc_cidr | CIDR block for the VPC | string | "10.0.0.0/16" |
| public_subnet_cidr | CIDR block for the public subnet | string | "10.0.1.0/24" |
| private_subnet_cidr | CIDR block for the private subnet | string | "10.0.2.0/24" |

## Outputs

| Name | Description |
|------|-------------|
| vpc_id | ID of the VPC |
| vpc_cidr_block | CIDR block of the VPC |
| public_subnet_id | ID of the public subnet |
| private_subnet_id | ID of the private subnet |
| public_subnet_cidr | CIDR block of the public subnet |
| private_subnet_cidr | CIDR block of the private subnet |
| internet_gateway_id | ID of the Internet Gateway |
| nat_gateway_id | ID of the NAT Gateway |
| public_route_table_id | ID of the public route table |
| private_route_table_id | ID of the private route table |

## Usage

```hcl
module "vpc" {
  source = "./modules/vpc"
  
  project_name         = "goodmeal"
  vpc_cidr            = "10.0.0.0/16"
  public_subnet_cidr  = "10.0.1.0/24"
  private_subnet_cidr = "10.0.2.0/24"
}
```

## Network Design

- **Public Subnet**: Suitable for resources that need direct internet access (load balancers, bastion hosts, NAT gateways)
- **Private Subnet**: Suitable for application servers, databases, and other backend resources that should not be directly accessible from the internet
- **Multi-AZ**: Subnets are placed in different availability zones for high availability 
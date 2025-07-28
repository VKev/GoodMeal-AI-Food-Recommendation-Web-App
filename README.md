# 🍽️ GoodMeal AI – AI Food Recommendation System

**GoodMeal AI** is a smart food recommendation platform that automatically suggests **food and nearby restaurants** based on your **mood and natural language input**. This is a **microservices-based project** featuring:

-  **AI-powered mood analysis**
-  **Restaurant location detection**
-  **Natural Language Processing (NLP)** for mood-based queries
-  **Next.js frontend** hosted on AWS Amplify
-  **.NET Microservices backend** deployed on AWS ECS & EC2 cluster

---

## 🧱 Project Stack

| Layer        | Tech Stack                               |
|--------------|-------------------------------------------|
| Frontend     | Next.js, TailwindCSS, Firebase Auth       |
| Backend      | ASP.NET Core Microservices (.NET 7)       |
| Infrastructure | AWS ECS, EC2, RDS, Redis, CloudFront, Amplify |
| CI/CD        | GitHub Actions, Docker Compose, Terraform |
| Database     | PostgreSQL                                |
| Messaging    | RabbitMQ, MassTransit                     |

---

## 📦 Deployment & Infrastructure Setup

### 🔥 Nuke All AWS Resources

```bash
aws-nuke nuke -c ./aws_nuke/nuke-config.yaml --profile terraform-user --no-dry-run --no-alias-check --no-prompt
```

### 🔐 Export CloudFront & EC2 Keys

```bash
terraform output -raw cloudfront_private_key_pem > cloudfront_private_key.pem
terraform output -raw ec2_private_key_pem > ec2_private_key.pem
```

### 🐳 Docker Production Build

```bash
docker compose -f 'docker-compose-production.yml' up -d --build
```

### 🛠️ Terraform Workflow

```bash
terraform init
terraform plan
terraform apply
```

---

## 🧪 Entity Framework Commands

### Add Migration

```bash
dotnet ef migrations add Migrations --msbuildprojectextensionspath "Build/obj" --project "../Infrastructure/Infrastructure.csproj" --startup-project "../WebApi/WebApi.csproj"
```

### Apply Database Updates

```bash
dotnet ef database update --msbuildprojectextensionspath .\Build\obj\ --project "../Infrastructure/Infrastructure.csproj" --startup-project "../WebApi/WebApi.csproj"
```

### 🧹 Clear PostgreSQL Database (Dev Reset)

```sql
DO $$ 
DECLARE
    r RECORD;
BEGIN
    FOR r IN (SELECT tablename FROM pg_tables WHERE schemaname = 'public') LOOP
        EXECUTE 'DROP TABLE IF EXISTS public.' || quote_ident(r.tablename) || ' CASCADE';
    END LOOP;
END $$;
```

---

## ☁️ Terraform Deployment Options

### Deploy Lambda@Edge + CloudFront (No Amplify)

```bash
terraform apply -var="deploy_amplify=false" -var="include_amplify_origins=false"
```

### Deploy Amplify Frontend

```bash
terraform apply -var="deploy_amplify=true" -var="include_amplify_origins=false"
```

### Include Amplify Origins in CloudFront

```bash
terraform apply -var="deploy_amplify=true" -var="include_amplify_origins=true"
```

---

## ⚙️ Terraform Variables

```hcl
variable "origin_domain_name" {
  description = "Domain name for CloudFront origin"
  type        = string
  default     = "s3.wasabisys.com"
}

variable "origin_path" {
  description = "Path for CloudFront origin"
  type        = string
  default     = "/khangstoragetest"
}
```

---

## Web UI Preview

### Mood-based Food Recommendation Interface

![Web UI Screenshot 1](https://github.com/user-attachments/assets/83e2f5d4-8044-4e96-a0cd-6f5a25c2c2d9)

### Restaurant Suggestions Nearby

![Web UI Screenshot 2](https://github.com/user-attachments/assets/4172a286-734a-427c-9ee7-fd67250982ac)




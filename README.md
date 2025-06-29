

aws-nuke nuke -c ./aws_nuke/nuke-config.yaml --profile terraform-user --no-dry-run --no-alias-check --no-prompt

terraform output -raw cloudfront_private_key_pem > cloudfront_private_key.pem
terraform output -raw ec2_private_key_pem > ec2_private_key.pem


docker compose -f 'docker-compose-production.yml' up -d --build



terraform init
terraform plan
terraform apply


Add

dotnet ef migrations add Migrations --msbuildprojectextensionspath "Build/obj" --project "../Infrastructure/Infrastructure.csproj" --startup-project "../WebApi/WebApi.csproj"


Update

dotnet ef database update --msbuildprojectextensionspath .\Build\obj\ --project "../Infrastructure/Infrastructure.csproj" --startup-project "../WebApi/WebApi.csproj"



clear database
DO $$ 
DECLARE
    r RECORD;
BEGIN
    -- Loop through all tables in the public schema and drop them
    FOR r IN (SELECT tablename FROM pg_tables WHERE schemaname = 'public') LOOP
        EXECUTE 'DROP TABLE IF EXISTS public.' || quote_ident(r.tablename) || ' CASCADE';
    END LOOP;
END $$;


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







#Deploy Lambda Edge and CloudFront
terraform apply -var="deploy_amplify=false" -var="include_amplify_origins=false"

#Deploy Amplify
terraform apply -var="deploy_amplify=true" -var="include_amplify_origins=false"

#Update CloudFront with Amplify Origins
terraform apply -var="deploy_amplify=true" -var="include_amplify_origins=true"


aws-nuke nuke -c ./aws_nuke/nuke-config.yaml --profile terraform-user --no-dry-run --no-alias-check --no-prompt

terraform output -raw cloudfront_private_key_pem > cloudfront_private_key.pem
terraform output -raw ec2_private_key_pem > ec2_private_key.pem


docker compose -f 'docker-compose-production.yml' up -d --build
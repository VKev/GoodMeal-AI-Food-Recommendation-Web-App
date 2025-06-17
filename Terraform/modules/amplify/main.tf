# Amplify App
resource "aws_amplify_app" "main" {
  name       = var.app_name
  repository = var.github_repository_url

  access_token = var.github_access_token

  build_spec = var.build_spec != "" ? var.build_spec : file("${path.module}/buildspec.yml")

  # Platform settings - WEB_COMPUTE for Next.js SSR
  platform = "WEB_COMPUTE"

  # Environment variables
  environment_variables = merge({
    "_LIVE_UPDATES" = "[{\"name\":\"Next.js\",\"pkg\":\"next-build\",\"type\":\"NPM\",\"version\":\"latest\"}]"
    "AMPLIFY_DIFF_DEPLOY" = "false"
    "AMPLIFY_MONOREPO_APP_ROOT" = "Frontend"
  }, var.environment_variables)

  enable_auto_branch_creation = var.enable_auto_branch_creation

  auto_branch_creation_config {
    enable_auto_build = var.enable_branch_auto_build
  }

  custom_rule {
    source = "/<*>"
    status = "404"
    target = "/404.html"
  }

  custom_rule {
    source = "/api/<*>"
    status = "200"
    target = "/api/<*>"
  }

  tags = var.tags
}

resource "aws_amplify_branch" "main" {
  app_id      = aws_amplify_app.main.id
  branch_name = var.branch_name

  framework = var.framework

  enable_auto_build = var.enable_branch_auto_build

  tags = var.tags
}

resource "aws_amplify_webhook" "main" {
  app_id      = aws_amplify_app.main.id
  branch_name = aws_amplify_branch.main.branch_name
  description = "Webhook for ${var.app_name} ${var.branch_name} branch"
}

# Get current AWS region
data "aws_region" "current" {}

# Auto-trigger initial deployment
resource "null_resource" "trigger_initial_deployment" {
  depends_on = [aws_amplify_branch.main]

  provisioner "local-exec" {
    command = "aws amplify start-job --app-id ${aws_amplify_app.main.id} --branch-name ${aws_amplify_branch.main.branch_name} --job-type RELEASE --region ${data.aws_region.current.name}"
  }

  triggers = {
    branch_name = aws_amplify_branch.main.branch_name
    app_id      = aws_amplify_app.main.id
  }
}

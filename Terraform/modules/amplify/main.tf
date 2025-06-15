# Amplify App
resource "aws_amplify_app" "main" {
  name       = var.app_name
  repository = var.github_repository_url

  access_token = var.github_access_token

  build_spec = var.build_spec != "" ? var.build_spec : yamlencode({
    version = 1
    applications = [
      {
        appRoot = "Frontend"
        frontend = {
          phases = {
            preBuild = {
              commands = [
                "echo 'Current directory:' && pwd",
                "echo 'Files in current directory:' && ls -la",
                "echo 'Checking package.json:' && cat package.json || echo 'No package.json found'",
                "npm install",
                "echo 'Files after npm install:' && ls -la",
                "echo 'Contents of node_modules/.bin:' && ls -la node_modules/.bin/ || echo 'node_modules/.bin not found'",
                "npm list next || echo 'Next.js not found in dependencies'",
                "npm list --depth=0 || echo 'Failed to list packages'"
              ]
            }
            build = {
              commands = [
                "echo 'Build phase - Current directory:' && pwd",
                "echo 'Build phase - Available files:' && ls -la",
                "echo 'Build phase - node_modules check:' && ls -la node_modules/.bin/ | head -10 || echo 'node_modules/.bin not accessible'",
                "export PATH=$PATH:./node_modules/.bin",
                "echo 'Build phase - PATH:' && echo $PATH",
                "npx --version && echo 'npx available'",
                "npx next build"
              ]
            }
            postBuild = {
              commands = [
                "echo 'PostBuild - Checking .next directory:' && ls -la .next/",
                "echo 'PostBuild - Looking for required-server-files.json:' && find . -name 'required-server-files.json' -type f || echo 'required-server-files.json not found'",
                "echo 'PostBuild - Copying additional files for SSR...'",
                "cp package.json .next/ 2>/dev/null || echo 'package.json copy failed or not needed'",
                "cp next.config.* .next/ 2>/dev/null || echo 'next.config copy failed or not found'",
                "echo 'PostBuild - Ensuring required files exist in .next'",
                "test -f .next/required-server-files.json || echo 'ERROR: required-server-files.json not found in .next directory'",
                "test -f .next/package.json || cp package.json .next/",
                "echo 'PostBuild - Final .next contents:' && ls -la .next/",
                "echo 'PostBuild - Verifying required-server-files.json exists:' && ls -la .next/required-server-files.json || echo 'required-server-files.json missing'",
                "echo 'PostBuild - Checking standalone directory:' && ls -la .next/standalone/ 2>/dev/null || echo 'No standalone directory found'"
              ]
            }
          }
          artifacts = {
            baseDirectory = ".next"
            files = [
              "**/*"
            ]
          }
          cache = {
            paths = [
              "node_modules/**/*",
              ".next/cache/**/*"
            ]
          }
        }
      }
    ]
  })

  # Platform settings
  platform = "WEB_COMPUTE"
  
  # Framework detection
  custom_headers = [
    {
      pattern = "/**"
      headers = {
        "X-Frame-Options" = "DENY"
        "X-Content-Type-Options" = "nosniff"
      }
    }
  ]

  # Environment variables
  environment_variables = var.environment_variables

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

  # Re-run if branch configuration changes
  triggers = {
    branch_name = aws_amplify_branch.main.branch_name
    app_id      = aws_amplify_app.main.id
  }
}

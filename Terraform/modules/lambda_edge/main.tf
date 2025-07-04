
resource "null_resource" "npm_install" {
  provisioner "local-exec" {
    command = "npm install"
    working_dir = "${path.module}"
  }
}

data "archive_file" "lambda_zip" {
  type        = "zip"
  source_dir  = "${path.module}"
  output_path = "${path.module}/lambda.zip"
}

resource "aws_ssm_parameter" "lambda_edge_secret" {
  name        = "/lambda/edge/secret"
  description = "Secret value for Lambda@Edge"
  type        = "SecureString"
  value       = var.lambda_edge_secret
  overwrite   = true
}

data "aws_caller_identity" "current" {}

resource "aws_iam_policy" "lambda_ssm_policy" {
  name        = "LambdaSSMPolicy"
  description = "Policy for Lambda@Edge to access SSM parameter for secret retrieval"
  policy = jsonencode({
    Version = "2012-10-17",
    Statement = [
      {
        Effect = "Allow",
        Action = [
          "ssm:GetParameter",
          "ssm:GetParameters",
          "ssm:GetParameterHistory"
        ],
        Resource = "arn:aws:ssm:us-east-1:${data.aws_caller_identity.current.account_id}:parameter/lambda/edge/secret"
      }
    ]
  })
}

resource "aws_iam_role_policy_attachment" "lambda_ssm_policy_attachment" {
  role       = aws_iam_role.lambda_edge_role.name
  policy_arn = aws_iam_policy.lambda_ssm_policy.arn
}


resource "aws_iam_role" "lambda_edge_role" {
  name = var.function_name

  assume_role_policy = jsonencode({
    Version = "2012-10-17",
    Statement = [
      {
        Action = "sts:AssumeRole",
        Principal = {
          "Service": ["lambda.amazonaws.com", "edgelambda.amazonaws.com"]
        },
        Effect = "Allow",
        Sid = ""
      }
    ]
  })
}

resource "aws_iam_role_policy_attachment" "lambda_edge_basic_execution" {
  role       = aws_iam_role.lambda_edge_role.name
  policy_arn = "arn:aws:iam::aws:policy/service-role/AWSLambdaBasicExecutionRole"
}

resource "aws_lambda_function" "this" {
  function_name    = var.function_name
  role             = aws_iam_role.lambda_edge_role.arn
  handler          = var.handler
  runtime          = var.runtime
  publish          = true
  filename         = data.archive_file.lambda_zip.output_path
  source_code_hash = data.archive_file.lambda_zip.output_base64sha256

  timeouts {
    delete = "30m"
  }

  depends_on = [ aws_ssm_parameter.lambda_edge_secret ]
}

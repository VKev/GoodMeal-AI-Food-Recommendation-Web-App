data "archive_file" "lambda_zip" {
  type        = "zip"
  source_dir  = "${path.module}"
  output_path = "${path.module}/lambda.zip"
}

resource "aws_iam_policy" "lambda_ssm_policy" {
  name        = "LambdaSSMPolicy"
  description = "Policy for Lambda@Edge to access SSM parameter for secret retrieval"
  policy = jsonencode({
    Version = "2012-10-17",
    Statement = [
      {
        Effect = "Allow",
        Action = [
          "ssm:GetParameter"
        ],
        Resource = "arn:aws:ssm:us-east-1:242201290212:parameter/lambda/edge/secret"
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
}

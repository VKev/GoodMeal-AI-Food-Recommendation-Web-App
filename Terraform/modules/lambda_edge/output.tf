output "lambda_function_arn" {
  description = "ARN of the Lambda function"
  value       = aws_lambda_function.this.arn
}

output "lambda_function_version" {
  description = "Version of the Lambda function"
  value       = aws_lambda_function.this.version
}

output "lambda_function_qualified_arn" {
  description = "Qualified ARN for Lambda@Edge (published version)"
  value       = aws_lambda_function.this.qualified_arn
}

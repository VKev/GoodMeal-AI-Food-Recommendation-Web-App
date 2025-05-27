output "cloudfront_distribution_id" {
  description = "The ID of the CloudFront distribution"
  value       = aws_cloudfront_distribution.this.id
}

output "cloudfront_domain_name" {
  description = "The domain name of the CloudFront distribution"
  value       = aws_cloudfront_distribution.this.domain_name
}

output "public_key_encoded" {
  description = "The encoded CloudFront public key"
  value       = aws_cloudfront_public_key.abda_lab_public_key.id
}
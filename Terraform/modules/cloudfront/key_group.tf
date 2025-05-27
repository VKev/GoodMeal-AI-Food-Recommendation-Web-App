# Create a CloudFront public key resource.
resource "tls_private_key" "cloudfront_key" {
  algorithm = "RSA"
  rsa_bits  = 2048 # CloudFront public keys for signed URLs/cookies typically use 2048-bit RSA keys.
}

resource "aws_cloudfront_public_key" "cloudfront_public_key" {
  name        = "${var.project_name}-Key-Group-PublicKey"
  encoded_key = tls_private_key.cloudfront_key.public_key_pem
  comment     = "Public key for ${var.project_name} key group"
}

# Create the CloudFront key group using the public key above.
resource "aws_cloudfront_key_group" "cloudfront_key_group" {
  name    = "Custom-${var.project_name}-Key-Group"
  items   = [aws_cloudfront_public_key.cloudfront_public_key.id]
  comment = "Key group for ${var.project_name}"

}

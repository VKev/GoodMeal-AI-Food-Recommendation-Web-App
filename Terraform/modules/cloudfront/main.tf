resource "aws_cloudfront_distribution" "this" {
  origin {
    domain_name = var.origin_domain_name
    origin_id   = "wasabisys.com"
    origin_path = var.origin_path

    custom_origin_config {
      http_port                = 80
      https_port               = 443
      origin_protocol_policy   = "https-only"
      origin_ssl_protocols     = ["TLSv1.2"]
      origin_read_timeout      = 30
      origin_keepalive_timeout = 5
    }

    custom_header {
      name  = "Referer"
      value = var.bucket_secret_referer
    }
  }

  enabled             = true
  is_ipv6_enabled     = true
  comment             = "CloudFront distribution using Wasabi"
  default_root_object = "index.html"

  default_cache_behavior {
    target_origin_id       = "wasabisys.com"
    viewer_protocol_policy = "redirect-to-https"
    compress               = true
    allowed_methods        = ["GET", "HEAD", "OPTIONS", "PUT", "POST", "PATCH", "DELETE"]
    cached_methods         = ["GET", "HEAD"]

    trusted_key_groups = [aws_cloudfront_key_group.abda_lab_key_group.id]

    cache_policy_id            = "658327ea-f89d-4fab-a63d-7e88639e58f6" # CachingOptimized
    origin_request_policy_id   = "b689b0a8-53d0-40ab-baf2-68738e2966ac" # AWS Managed AllViewerExceptHostHeader
    response_headers_policy_id = aws_cloudfront_response_headers_policy.cors_and_security_policy.id
  }

  # Additional behavior for /public* path
  ordered_cache_behavior {
    path_pattern           = "/public*"
    target_origin_id       = "wasabisys.com"
    viewer_protocol_policy = "redirect-to-https"
    compress               = true
    allowed_methods        = ["GET", "HEAD", "OPTIONS"]
    cached_methods         = ["GET", "HEAD"]

    # No trusted key groups here.
    cache_policy_id            = "658327ea-f89d-4fab-a63d-7e88639e58f6" # CachingOptimized
    origin_request_policy_id   = "b689b0a8-53d0-40ab-baf2-68738e2966ac" # AWS Managed AllViewerExceptHostHeader
    response_headers_policy_id = aws_cloudfront_response_headers_policy.cors_and_security_policy.id
  }

  # Additional behavior for /set-cookie* path
  ordered_cache_behavior {
    path_pattern           = "/set-cookie*"
    target_origin_id       = "wasabisys.com"
    viewer_protocol_policy = "redirect-to-https"
    # Per the settings, compress is not enabled for this behavior.
    compress        = false
    allowed_methods = ["GET", "HEAD", "OPTIONS", "PUT", "POST", "PATCH", "DELETE"]
    cached_methods  = ["GET", "HEAD"]

    # Use the cache policy that honors origin Cache-Control headers.
    cache_policy_id = "83da9c7e-98b4-4e11-a168-04f0df8e2c65"
    # Use an origin request policy that forwards all viewer parameters.
    origin_request_policy_id   = "216adef6-5c7f-47e4-b989-5492eafa07d3"
    response_headers_policy_id = aws_cloudfront_response_headers_policy.cors_and_security_policy.id

    lambda_function_association {
      event_type   = "viewer-request"
      lambda_arn   = var.set_cookie_lambda_arn
      include_body = false
    }
  }

  price_class = "PriceClass_100"

  restrictions {
    geo_restriction {
      restriction_type = "none"
    }
  }

  viewer_certificate {
    cloudfront_default_certificate = true
  }
}

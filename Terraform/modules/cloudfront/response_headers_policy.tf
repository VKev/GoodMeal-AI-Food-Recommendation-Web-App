resource "aws_cloudfront_response_headers_policy" "cors_and_security_policy" {
  name    = "Custom-CORS-and-Security-Policy"
  comment = "Policy for enabling CORS and adding security headers"

  cors_config {
    access_control_allow_credentials = true

    access_control_allow_headers {
      items = [
        "Content-Type",
        "Authorization",
        "X-Amz-Date",
        "X-Amz-Security-Token",
        "X-Amz-Content-Sha256",
        "Accept",
        "Content-Disposition",
      ]
    }

    access_control_allow_methods {
      items = [
        "GET",
        "HEAD",
        "POST",
        "PUT",
        "PATCH",
        "DELETE",
        "OPTIONS",
      ]
    }

    access_control_allow_origins {
      items = [
        "http://localhost:3000",
        "localhost:3000",
      ]
    }

    access_control_expose_headers {
      items = [
        "Content-Disposition",
        "Content-Length",
        "Accept-Ranges",
        "ETag",
        "Last-Modified",
        "Cache-Control",
        "Content-Type",
        "Referer",
      ]
    }

    access_control_max_age_sec = 600
    origin_override            = true
  }

  security_headers_config {
    content_security_policy {
      override                = true
      content_security_policy = "default-src 'self';"
    }
    content_type_options {
      override = true
    }
    frame_options {
      override     = true
      frame_option = "DENY"
    }
    referrer_policy {
      override        = true
      referrer_policy = "no-referrer"
    }
    strict_transport_security {
      override                = true
      access_control_max_age_sec = 63072000
      include_subdomains      = true
      preload                 = true
    }
    xss_protection {
      override   = true
      protection = true
      mode_block = true
    }
  }
}

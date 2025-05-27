terraform {
  required_providers {
    aws = {
      source  = "hashicorp/aws"
      version = "~> 5.87"
    }
    tls = {
      source  = "hashicorp/tls"
      version = "~> 4.0"
    }
  }

  backend "s3" {
    bucket = "khangstoragetest"
    key    = "terraform/state.tfstate"
    region = "ap-southeast-1"
    endpoints = {
      s3 = "https://s3.ap-southeast-1.wasabisys.com"
    }
    profile                     = "wasabi-user"
    use_path_style              = true
    skip_credentials_validation = true
    skip_requesting_account_id  = true
  }
}


provider "aws" {
  region  = "us-east-1"
  profile = "terraform-user"

}

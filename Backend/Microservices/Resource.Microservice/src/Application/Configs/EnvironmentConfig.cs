using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Configs
{
    public class EnvironmentConfig: SharedLibrary.Configs.EnvironmentConfig
    {
        public string CloudFrontDistributionDomain => Environment.GetEnvironmentVariable("AWS_CLOUD_FRONT_DISTRIBUTION_DOMAIN") ?? "default";
        public string CloudFrontPrivateKey => Environment.GetEnvironmentVariable("AWS_CLOUD_FRONT_PRIVATE_KEY") ?? "default";
        public string CloudFrontKeyId => Environment.GetEnvironmentVariable("AWS_CLOUD_FRONT_KEY_ID") ?? "default";
        public string SetCookieEdgeFunctionSecret => Environment.GetEnvironmentVariable("LAMBDA_EDGE_SECRET") ?? "default";
        public string AwsRoleArn => Environment.GetEnvironmentVariable("AWS_ROLE_ARN") ?? "default";
        public string AwsAccessKeyUser => Environment.GetEnvironmentVariable("AWS_ACCESS_KEY_USER") ?? "default";
        public string AwsSecretKeyUser => Environment.GetEnvironmentVariable("AWS_SECRET_KEY_USER") ?? "default";
    }
}
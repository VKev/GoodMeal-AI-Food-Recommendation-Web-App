using Amazon.CloudFront;
using Amazon.S3;
using Amazon.SecurityToken;
using Application.Abstractions.Messaging;
using Application.Configs;
using SharedLibrary.Common.ResponseModel;
using System.Security.Cryptography;
using System.Text;

namespace Application.Features.Aws.Commands
{
    public sealed record CreateCloudFrontSignedCookieCommand(
        string ResourceUrl,
        int ExpiryHour
    ) : ICommand<Dictionary<string, string>>;

    internal sealed class CreateCloudFrontSignedCookieCommandHandler
        : ICommandHandler<CreateCloudFrontSignedCookieCommand, Dictionary<string, string>>
    {
        private readonly IAmazonS3 _s3Client;
        private readonly EnvironmentConfig _config;
        private readonly IAmazonSecurityTokenService _stsClient;

        public CreateCloudFrontSignedCookieCommandHandler(
            EnvironmentConfig config,
            IAmazonS3 s3Client,
            IAmazonSecurityTokenService stsClient)
        {
            _config = config;
            _s3Client = s3Client;
            _stsClient = stsClient;
        }

        public async Task<Result<Dictionary<string, string>>> Handle(
            CreateCloudFrontSignedCookieCommand command,
            CancellationToken cancellationToken)
        {
            var cookies = AmazonCloudFrontCookieSigner.GetCookiesForCustomPolicy(
                AmazonCloudFrontCookieSigner.Protocols.Https,
                _config.CloudFrontDistributionDomain,
                new StringReader(_config.CloudFrontPrivateKey),
                command.ResourceUrl,
                _config.CloudFrontKeyId,
                DateTime.UtcNow.AddHours(command.ExpiryHour),
                DateTime.UtcNow,
                "0.0.0.0/0"
            );

            var policyValue    = cookies.Policy.Value;
            var signatureValue = cookies.Signature.Value;
            var keyPairValue   = cookies.KeyPairId.Value;

            var nowSeconds = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            var dataToSign = $"{policyValue}:{signatureValue}:{keyPairValue}:{nowSeconds}";

            string authValue;
            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_config.SetCookieEdgeFunctionSecret)))
            {
                var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(dataToSign));
                authValue = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }

            var resultDict = new Dictionary<string, string>
            {
                { cookies.Policy.Key, policyValue },
                { cookies.Signature.Key, signatureValue },
                { cookies.KeyPairId.Key, keyPairValue },
                { "auth", authValue },
                { "ts", nowSeconds.ToString() }
            };

            return resultDict;
        }
    }
}

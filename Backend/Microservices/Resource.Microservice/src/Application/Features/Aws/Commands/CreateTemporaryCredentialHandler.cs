using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.SecurityToken;
using Amazon.SecurityToken.Model;
using SharedLibrary.Common.Messaging;
using Application.Configs;
using SharedLibrary.Common.ResponseModel;

namespace Application.Features.Aws.Commands
{
    public sealed record CreateTemporaryCredentialCommand(
        string Name,
        int DurationSeconds
    ) : ICommand<Credentials>;

    internal sealed class CreateTemporaryCredentialCommandHandler : ICommandHandler<CreateTemporaryCredentialCommand, Credentials>
    {
        private readonly IAmazonS3 _s3Client;

        private readonly EnvironmentConfig _config;

        private readonly IAmazonSecurityTokenService _stsClient; 

        public CreateTemporaryCredentialCommandHandler(EnvironmentConfig config,IAmazonS3 s3Client, IAmazonSecurityTokenService stsClient)
        {
            _config = config;
            _s3Client = s3Client;
            _stsClient = stsClient;
        }

        public async Task<Result<Credentials>> Handle(CreateTemporaryCredentialCommand command, CancellationToken cancellationToken)
        {

            var request = new AssumeRoleRequest
            {
                RoleArn = _config.AwsRoleArn, 
                RoleSessionName = command.Name, 
                DurationSeconds = command.DurationSeconds
            };

            var response = await _stsClient.AssumeRoleAsync(request);
            return response.Credentials;
        }
    }
}
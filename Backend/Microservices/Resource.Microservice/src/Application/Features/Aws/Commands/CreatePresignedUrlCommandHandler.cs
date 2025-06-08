using Amazon.S3;
using Amazon.S3.Model;
using System;
using Application.Abstractions.Messaging;
using SharedLibrary.Common.ResponseModel;

namespace Application.Features.Aws.Commands
{
    public sealed record CreatePresignedUrlCommand(
    string BucketName,
    string ObjectKey,
    TimeSpan ExpiryDuration,
    HttpVerb HttpVerb
    ) : ICommand<string>;

    internal sealed class CreatePresignedUrlCommandHandler : ICommandHandler<CreatePresignedUrlCommand, string>
    {
        private readonly IAmazonS3 _s3Client;

        public CreatePresignedUrlCommandHandler(IAmazonS3 s3Client)
        {
            _s3Client = s3Client;
        }

        public Task<Result<string>> Handle(CreatePresignedUrlCommand command, CancellationToken cancellationToken)
        {

            var request = new GetPreSignedUrlRequest
            {
                BucketName = command.BucketName,
                Key = command.ObjectKey,
                Expires = DateTime.UtcNow.Add(command.ExpiryDuration),
                Verb = command.HttpVerb
            };

            var presignedUrl =  _s3Client.GetPreSignedURL(request);
            return Task.FromResult(Result.Success(presignedUrl));
        }
    }
}
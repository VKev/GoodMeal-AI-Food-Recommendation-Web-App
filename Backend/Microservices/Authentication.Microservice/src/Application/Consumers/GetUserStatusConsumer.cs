using FirebaseAdmin;
using FirebaseAdmin.Auth;
using MassTransit;
using Microsoft.Extensions.Logging;
using SharedLibrary.Contracts.Authentication;

namespace Application.Consumers;

public class GetUserStatusConsumer : IConsumer<GetUserStatusRequest>
{
    private readonly ILogger<GetUserStatusConsumer> _logger;

    public GetUserStatusConsumer(ILogger<GetUserStatusConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<GetUserStatusRequest> context)
    {
        try
        {
            var userRecord = await FirebaseAuth.DefaultInstance.GetUserAsync(context.Message.IdentityId, context.CancellationToken);

            var response = new GetUserStatusResponse
            {
                IdentityId = userRecord.Uid,
                Email = userRecord.Email ?? "",
                Name = userRecord.DisplayName ?? "",
                IsDisabled = userRecord.Disabled,
                EmailVerified = userRecord.EmailVerified,
                LastSignInTime = userRecord.UserMetaData?.LastSignInTimestamp,
                CreationTime = userRecord.UserMetaData?.CreationTimestamp
            };

            await context.RespondAsync(response);
        }
        catch (FirebaseAuthException ex)
        {
            _logger.LogError(ex, "Firebase error getting user status for {IdentityId}", context.Message.IdentityId);
            
            await context.RespondAsync(new GetUserStatusResponse 
            { 
                IdentityId = context.Message.IdentityId,
                Email = "",
                Name = "",
                IsDisabled = false,
                EmailVerified = false,
                LastSignInTime = null,
                CreationTime = null
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error getting user status for {IdentityId}", context.Message.IdentityId);
            throw;
        }
    }
} 
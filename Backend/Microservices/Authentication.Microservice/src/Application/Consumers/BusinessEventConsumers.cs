using FirebaseAdmin;
using FirebaseAdmin.Auth;
using MassTransit;
using Microsoft.Extensions.Logging;
using SharedLibrary.Common.Event;
using Newtonsoft.Json.Linq;

namespace Application.Consumers;

public class BusinessActivatedEventConsumer : IConsumer<BusinessActivatedEvent>
{
    private readonly ILogger<BusinessActivatedEventConsumer> _logger;

    public BusinessActivatedEventConsumer(ILogger<BusinessActivatedEventConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<BusinessActivatedEvent> context)
    {
        try
        {
            var @event = context.Message;
            
            _logger.LogInformation("Processing BusinessActivatedEvent for business {BusinessId} and owner {OwnerId}", 
                @event.BusinessId, @event.OwnerId);

            // Lấy thông tin user từ Firebase
            var userRecord = await FirebaseAuth.DefaultInstance.GetUserAsync(@event.OwnerId, context.CancellationToken);

            var currentRoles = new List<string>();
            if (userRecord.CustomClaims != null && userRecord.CustomClaims.TryGetValue("roles", out var rolesObj))
            {
                if (rolesObj is string[] roleArray)
                {
                    currentRoles = roleArray.ToList();
                }
                else if (rolesObj is JArray jArray)
                {
                    currentRoles = jArray.ToObject<string[]>()?.ToList() ?? new List<string>();
                }
                else if (rolesObj is List<string> roleList)
                {
                    currentRoles = roleList;
                }
                else if (rolesObj is string singleRole)
                {
                    currentRoles = new List<string> { singleRole };
                }
            }

            // Kiểm tra và thêm role "Business" nếu chưa có
            if (!currentRoles.Contains("Business"))
            {
                currentRoles.Add("Business");

                var customClaims = new Dictionary<string, object>();
                if (userRecord.CustomClaims != null)
                {
                    foreach (var claim in userRecord.CustomClaims)
                    {
                        customClaims[claim.Key] = claim.Value;
                    }
                }

                customClaims["roles"] = currentRoles.ToArray();
                customClaims["updated_at"] = DateTime.UtcNow.ToString("O");

                await FirebaseAuth.DefaultInstance.SetCustomUserClaimsAsync(@event.OwnerId, customClaims, context.CancellationToken);
                await FirebaseAuth.DefaultInstance.RevokeRefreshTokensAsync(@event.OwnerId, context.CancellationToken);

                _logger.LogInformation("Successfully added Business role to user {OwnerId} for business {BusinessId}", 
                    @event.OwnerId, @event.BusinessId);
            }
            else
            {
                _logger.LogInformation("User {OwnerId} already has Business role", @event.OwnerId);
            }
        }
        catch (FirebaseAuthException ex)
        {
            _logger.LogError(ex, "Firebase error adding Business role to user {OwnerId} for business {BusinessId}", 
                context.Message.OwnerId, context.Message.BusinessId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error processing BusinessActivatedEvent for business {BusinessId} and owner {OwnerId}", 
                context.Message.BusinessId, context.Message.OwnerId);
        }
    }
}

public class BusinessDeactivatedEventConsumer : IConsumer<BusinessDeactivatedEvent>
{
    private readonly ILogger<BusinessDeactivatedEventConsumer> _logger;

    public BusinessDeactivatedEventConsumer(ILogger<BusinessDeactivatedEventConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<BusinessDeactivatedEvent> context)
    {
        try
        {
            var @event = context.Message;
            
            _logger.LogInformation("Processing BusinessDeactivatedEvent for business {BusinessId} and owner {OwnerId}", 
                @event.BusinessId, @event.OwnerId);

            // Lấy thông tin user từ Firebase
            var userRecord = await FirebaseAuth.DefaultInstance.GetUserAsync(@event.OwnerId, context.CancellationToken);

            var currentRoles = new List<string>();
            if (userRecord.CustomClaims != null && userRecord.CustomClaims.TryGetValue("roles", out var rolesObj))
            {
                if (rolesObj is string[] roleArray)
                {
                    currentRoles = roleArray.ToList();
                }
                else if (rolesObj is JArray jArray)
                {
                    currentRoles = jArray.ToObject<string[]>()?.ToList() ?? new List<string>();
                }
                else if (rolesObj is List<string> roleList)
                {
                    currentRoles = roleList;
                }
                else if (rolesObj is string singleRole)
                {
                    currentRoles = new List<string> { singleRole };
                }
            }

            // Kiểm tra và xóa role "Business" nếu có
            if (currentRoles.Contains("Business"))
            {
                currentRoles.Remove("Business");

                var customClaims = new Dictionary<string, object>();
                if (userRecord.CustomClaims != null)
                {
                    foreach (var claim in userRecord.CustomClaims)
                    {
                        customClaims[claim.Key] = claim.Value;
                    }
                }

                customClaims["roles"] = currentRoles.ToArray();
                customClaims["updated_at"] = DateTime.UtcNow.ToString("O");

                await FirebaseAuth.DefaultInstance.SetCustomUserClaimsAsync(@event.OwnerId, customClaims, context.CancellationToken);
                await FirebaseAuth.DefaultInstance.RevokeRefreshTokensAsync(@event.OwnerId, context.CancellationToken);

                _logger.LogInformation("Successfully removed Business role from user {OwnerId} for business {BusinessId}", 
                    @event.OwnerId, @event.BusinessId);
            }
            else
            {
                _logger.LogInformation("User {OwnerId} does not have Business role to remove", @event.OwnerId);
            }
        }
        catch (FirebaseAuthException ex)
        {
            _logger.LogError(ex, "Firebase error removing Business role from user {OwnerId} for business {BusinessId}", 
                context.Message.OwnerId, context.Message.BusinessId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error processing BusinessDeactivatedEvent for business {BusinessId} and owner {OwnerId}", 
                context.Message.BusinessId, context.Message.OwnerId);
        }
    }
} 
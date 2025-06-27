using MassTransit;
using Microsoft.Extensions.Logging;
using SharedLibrary.Common.Messaging;
using SharedLibrary.Common.ResponseModel;
using SharedLibrary.Contracts.Business;

namespace Application.Admin.Commands.BusinessCommand;

public sealed record ActiveBusinessCommand(Guid BusinessId) : ICommand<ActiveBusinessResponse>;

internal sealed class EnableBusinessCommandHandler : ICommandHandler<ActiveBusinessCommand, ActiveBusinessResponse>
{
    private readonly IRequestClient<ActiveBusinessRequest> _requestClient;
    private readonly ILogger<EnableBusinessCommandHandler> _logger;

    public EnableBusinessCommandHandler(
        IRequestClient<ActiveBusinessRequest> requestClient,
        ILogger<EnableBusinessCommandHandler> logger)
    {
        _requestClient = requestClient;
        _logger = logger;
    }

    public async Task<Result<ActiveBusinessResponse>> Handle(ActiveBusinessCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Requesting to enable business {BusinessId}", request.BusinessId);

            var response = await _requestClient.GetResponse<ActiveBusinessResponse>(
                new ActiveBusinessRequest
                {
                    BusinessId = request.BusinessId
                }, cancellationToken);

            if (!response.Message.IsSuccess)
            {
                _logger.LogWarning("Failed to active business {BusinessId}: {ErrorMessage}", request.BusinessId,
                    response.Message.ErrorMessage);
                return Result.Failure<ActiveBusinessResponse>(new Error("BusinessService.Error",
                    response.Message.ErrorMessage ?? "Failed to enable business"));
            }


            _logger.LogInformation("Successfully active business {BusinessId}", response.Message.Business?.Id);
            return Result.Success(response.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while enabling business {BusinessId}", request.BusinessId);
            return Result.Failure<ActiveBusinessResponse>(new Error("InternalError", "An unexpected error occurred"));
        }
    }
}
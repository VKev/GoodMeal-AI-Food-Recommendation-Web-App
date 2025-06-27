using MassTransit;
using Microsoft.Extensions.Logging;
using SharedLibrary.Common.Messaging;
using SharedLibrary.Common.ResponseModel;
using SharedLibrary.Contracts.Business;

namespace Application.Admin.Commands.BusinessCommand.ActiveBusinessCommand;

public sealed record DeactiveBusinessCommand(Guid BusinessId) : ICommand<DeactiveBusinessResponse>;

internal sealed class
    DeactiveBusinessCommandHandler : ICommandHandler<DeactiveBusinessCommand, DeactiveBusinessResponse>
{
    private readonly IRequestClient<DeactiveBusinessRequest> _requestClient;
    private readonly ILogger<DeactiveBusinessCommandHandler> _logger;

    public DeactiveBusinessCommandHandler(
        IRequestClient<DeactiveBusinessRequest> requestClient,
        ILogger<DeactiveBusinessCommandHandler> logger)
    {
        _requestClient = requestClient;
        _logger = logger;
    }

    public async Task<Result<DeactiveBusinessResponse>> Handle(DeactiveBusinessCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Requesting to enable business {BusinessId}", request.BusinessId);

            var response = await _requestClient.GetResponse<DeactiveBusinessResponse>(
                new DeactiveBusinessRequest
                {
                    BusinessId = request.BusinessId
                }, cancellationToken);

            if (!response.Message.IsSuccess)
            {
                _logger.LogWarning("Failed to deactive business {BusinessId}: {ErrorMessage}", request.BusinessId,
                    response.Message.ErrorMessage);

                return Result.Failure<DeactiveBusinessResponse>(new Error("BusinessService.Error",
                    response.Message.ErrorMessage ?? "Failed to enable business"));
            }


            _logger.LogInformation("Successfully deactive business {BusinessId}", response.Message.Business?.Id);
            return Result.Success(response.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while enabling business {BusinessId}", request.BusinessId);
            return Result.Failure<DeactiveBusinessResponse>(new Error("InternalError", "An unexpected error occurred"));
        }
    }
}
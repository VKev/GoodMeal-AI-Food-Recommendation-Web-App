using MassTransit;
using SharedLibrary.Common.Messaging;
using SharedLibrary.Common.ResponseModel;
using SharedLibrary.Contracts.Business;
using Microsoft.Extensions.Logging;

namespace Application.Admin.Queries.GetAllBusinessesQuery;

public sealed record GetAllBusinessesQuery() : IQuery<GetAllBusinessesResponse>;

public sealed record GetAllBusinessesResponse(
    IEnumerable<BusinessDto> Businesses
);

internal sealed class GetAllBusinessesQueryHandler : IQueryHandler<GetAllBusinessesQuery, GetAllBusinessesResponse>
{
    private readonly IRequestClient<GetAllBusinessesRequest> _requestClient;
    private readonly ILogger<GetAllBusinessesQueryHandler> _logger;

    public GetAllBusinessesQueryHandler(
        IRequestClient<GetAllBusinessesRequest> requestClient,
        ILogger<GetAllBusinessesQueryHandler> logger)
    {
        _requestClient = requestClient;
        _logger = logger;
    }

    public async Task<Result<GetAllBusinessesResponse>> Handle(GetAllBusinessesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Requesting all businesses from Business microservice");

            var response = await _requestClient.GetResponse<SharedLibrary.Contracts.Business.GetAllBusinessesResponse>(
                new GetAllBusinessesRequest(), cancellationToken);

            if (!response.Message.IsSuccess)
            {
                _logger.LogWarning("Failed to get businesses: {ErrorMessage}", response.Message.ErrorMessage);
                return Result.Failure<GetAllBusinessesResponse>(new Error("BusinessService.Error", response.Message.ErrorMessage ?? "Failed to retrieve businesses"));
            }

            var result = new GetAllBusinessesResponse(response.Message.Businesses);
            
            _logger.LogInformation("Successfully retrieved {Count} businesses", response.Message.Businesses.Count());
            return Result.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting all businesses");
            return Result.Failure<GetAllBusinessesResponse>(new Error("InternalError", "An unexpected error occurred"));
        }
    }
} 
using FluentValidation;
using Microsoft.Extensions.Logging;
using SharedLibrary.Common.Messaging;
using SharedLibrary.Common.ResponseModel;
using Domain.Repositories;
using AutoMapper;

namespace Application.Subscriptions.Queries.GetAllSubscriptionsQuery;

public sealed record GetAllSubscriptionsQuery() : IQuery<GetAllSubscriptionsResponse>;

public sealed record SubscriptionInfo(
    Guid Id,
    string Name,
    string? Description,
    decimal Price,
    int DurationInMonths,
    string Currency,
    bool IsActive,
    DateTime? CreatedAt,
    DateTime? UpdatedAt
);

public sealed record GetAllSubscriptionsResponse(
    IEnumerable<SubscriptionInfo> Subscriptions,
    int TotalCount
);

internal sealed class GetAllSubscriptionsQueryHandler : IQueryHandler<GetAllSubscriptionsQuery, GetAllSubscriptionsResponse>
{
    private readonly ILogger<GetAllSubscriptionsQueryHandler> _logger;
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly IMapper _mapper;

    public GetAllSubscriptionsQueryHandler(ILogger<GetAllSubscriptionsQueryHandler> logger, ISubscriptionRepository subscriptionRepository, IMapper mapper)
    {
        _logger = logger;
        _subscriptionRepository = subscriptionRepository;
        _mapper = mapper;
    }

    public async Task<Result<GetAllSubscriptionsResponse>> Handle(GetAllSubscriptionsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var subscriptions = await _subscriptionRepository.GetAllAsync();
            
            var subscriptionInfos = _mapper.Map<IEnumerable<SubscriptionInfo>>(subscriptions);

            var response = new GetAllSubscriptionsResponse(subscriptionInfos, subscriptionInfos.Count());

            _logger.LogInformation("Successfully retrieved {Count} subscriptions", subscriptionInfos.Count());
            return Result.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error retrieving subscriptions");
            return Result.Failure<GetAllSubscriptionsResponse>(new Error("InternalError", "An unexpected error occurred"));
        }
    }
} 
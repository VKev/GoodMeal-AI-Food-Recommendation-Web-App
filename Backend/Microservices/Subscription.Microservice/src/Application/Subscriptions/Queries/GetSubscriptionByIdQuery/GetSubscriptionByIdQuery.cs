using FluentValidation;
using Microsoft.Extensions.Logging;
using SharedLibrary.Common.Messaging;
using SharedLibrary.Common.ResponseModel;
using Domain.Repositories;
using AutoMapper;

namespace Application.Subscriptions.Queries.GetSubscriptionByIdQuery;

public sealed record GetSubscriptionByIdQuery(Guid SubscriptionId) : IQuery<GetSubscriptionByIdResponse>;

public sealed record GetSubscriptionByIdResponse(
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

internal sealed class GetSubscriptionByIdQueryHandler : IQueryHandler<GetSubscriptionByIdQuery, GetSubscriptionByIdResponse>
{
    private readonly ILogger<GetSubscriptionByIdQueryHandler> _logger;
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly IMapper _mapper;

    public GetSubscriptionByIdQueryHandler(ILogger<GetSubscriptionByIdQueryHandler> logger, ISubscriptionRepository subscriptionRepository, IMapper mapper)
    {
        _logger = logger;
        _subscriptionRepository = subscriptionRepository;
        _mapper = mapper;
    }

    public async Task<Result<GetSubscriptionByIdResponse>> Handle(GetSubscriptionByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var subscription = await _subscriptionRepository.GetByIdAsync(request.SubscriptionId);
            
            if (subscription is null)
            {
                _logger.LogWarning("Subscription with ID {SubscriptionId} not found", request.SubscriptionId);
                return Result.Failure<GetSubscriptionByIdResponse>(new Error("Subscription.NotFound", "Subscription not found"));
            }

            var response = _mapper.Map<GetSubscriptionByIdResponse>(subscription);

            _logger.LogInformation("Successfully retrieved subscription {SubscriptionId}", request.SubscriptionId);
            return Result.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error retrieving subscription {SubscriptionId}", request.SubscriptionId);
            return Result.Failure<GetSubscriptionByIdResponse>(new Error("InternalError", "An unexpected error occurred"));
        }
    }
}

public class GetSubscriptionByIdQueryValidator : AbstractValidator<GetSubscriptionByIdQuery>
{
    public GetSubscriptionByIdQueryValidator()
    {
        RuleFor(x => x.SubscriptionId)
            .NotEmpty()
            .WithMessage("Subscription ID is required");
    }
} 
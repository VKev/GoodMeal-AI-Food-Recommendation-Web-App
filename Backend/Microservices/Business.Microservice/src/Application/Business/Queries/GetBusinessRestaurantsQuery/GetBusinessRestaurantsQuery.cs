using Domain.Entities;
using FluentValidation;
using Microsoft.Extensions.Logging;
using SharedLibrary.Common.Messaging;
using SharedLibrary.Common.ResponseModel;
using Domain.Repositories;
using AutoMapper;

namespace Application.Business.Queries.GetBusinessRestaurantsQuery;

public sealed record GetBusinessRestaurantsQuery(Guid BusinessId) : IQuery<GetBusinessRestaurantsResponse>;

public sealed record BusinessRestaurantInfo(
    Guid Id,
    Guid BusinessId,
    Guid RestaurantId,
    RestaurantInfo? Restaurant,
    DateTime? CreatedAt,
    bool? IsDisable
);

public sealed record GetBusinessRestaurantsResponse(
    Guid BusinessId,
    IEnumerable<BusinessRestaurantInfo> Restaurants,
    int TotalCount
);

internal sealed class
    GetBusinessRestaurantsQueryHandler : IQueryHandler<GetBusinessRestaurantsQuery, GetBusinessRestaurantsResponse>
{
    private readonly ILogger<GetBusinessRestaurantsQueryHandler> _logger;
    private readonly IBusinessRestaurantRepository _businessRestaurantRepository;
    private readonly IBusinessRepository _businessRepository;
    private readonly IRestaurantRepository _restaurantRepository;
    private readonly IMapper _mapper;

    public GetBusinessRestaurantsQueryHandler(
        ILogger<GetBusinessRestaurantsQueryHandler> logger,
        IBusinessRestaurantRepository businessRestaurantRepository,
        IBusinessRepository businessRepository,
        IRestaurantRepository restaurantRepository,
        IMapper mapper)
    {
        _logger = logger;
        _businessRestaurantRepository = businessRestaurantRepository;
        _businessRepository = businessRepository;
        _restaurantRepository = restaurantRepository;
        _mapper = mapper;
    }

    public async Task<Result<GetBusinessRestaurantsResponse>> Handle(GetBusinessRestaurantsQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var businessExists = await _businessRepository.ExistsAsync(request.BusinessId);
            if (!businessExists)
            {
                _logger.LogWarning("Business with ID {BusinessId} not found", request.BusinessId);
                return Result.Failure<GetBusinessRestaurantsResponse>(new Error("Business.NotFound",
                    "Business not found"));
            }

            var businessRestaurants =
                await _businessRestaurantRepository.GetByBusinessIdAsync(request.BusinessId, cancellationToken);

            if (!businessRestaurants.Any())
            {
                var emptyResponse = new GetBusinessRestaurantsResponse(request.BusinessId,
                    Enumerable.Empty<BusinessRestaurantInfo>(), 0);
                return Result.Success(emptyResponse);
            }

            var restaurantIds = businessRestaurants.Select(br => br.RestaurantId);
            var restaurantsResult =
                await _restaurantRepository.GetRestaurantsByIdsAsync(restaurantIds, cancellationToken);

            var restaurantInfos = new List<BusinessRestaurantInfo>();

            foreach (var businessRestaurant in businessRestaurants)
            {
                var restaurantInfo = restaurantsResult.FirstOrDefault(r => r.Id == businessRestaurant.RestaurantId);

                var businessRestaurantInfo = _mapper.Map<BusinessRestaurantInfo>(businessRestaurant);
                businessRestaurantInfo = businessRestaurantInfo with { Restaurant = restaurantInfo };

                restaurantInfos.Add(businessRestaurantInfo);
            }

            var response = new GetBusinessRestaurantsResponse(
                request.BusinessId,
                restaurantInfos,
                restaurantInfos.Count
            );

            _logger.LogInformation("Successfully retrieved {Count} restaurants for business {BusinessId}",
                restaurantInfos.Count, request.BusinessId);
            return Result.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error getting restaurants for business {BusinessId}", request.BusinessId);
            return Result.Failure<GetBusinessRestaurantsResponse>(new Error("InternalError",
                "An unexpected error occurred"));
        }
    }
}

public class GetBusinessRestaurantsQueryValidator : AbstractValidator<GetBusinessRestaurantsQuery>
{
    public GetBusinessRestaurantsQueryValidator()
    {
        RuleFor(x => x.BusinessId)
            .NotEmpty()
            .WithMessage("Business ID is required");
    }
}
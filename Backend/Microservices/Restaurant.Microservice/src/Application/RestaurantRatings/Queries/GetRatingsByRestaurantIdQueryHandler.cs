using SharedLibrary.Common.Messaging;
using SharedLibrary.Common.ResponseModel;
using AutoMapper;
using Domain.Repositories;

namespace Application.RestaurantRatings.Queries;

public sealed record GetRatingByRestaurantIdQuery(Guid RestaurantId) : IQuery<IEnumerable<GetRatingResponse>>;

internal sealed class GetRatingByRestaurantIdQueryHandler : IQueryHandler<GetRatingByRestaurantIdQuery, IEnumerable<GetRatingResponse>>
{
    private readonly IRestaurantRatingRepository _restaurantRatingRepository;
    private readonly IMapper _mapper;

    public GetRatingByRestaurantIdQueryHandler(IRestaurantRatingRepository restaurantRatingRepository, IMapper mapper)
    {
        _restaurantRatingRepository = restaurantRatingRepository;
        _mapper = mapper;
    }
    public async Task<Result<IEnumerable<GetRatingResponse>>> Handle(GetRatingByRestaurantIdQuery request, CancellationToken cancellationToken)
    {
        var rating = await _restaurantRatingRepository.GetByRestaurantIdAsync(request.RestaurantId, cancellationToken);

        var ratingResponses = _mapper.Map<IEnumerable<GetRatingResponse>>(rating);
        return Result.Success(ratingResponses);
    }
}
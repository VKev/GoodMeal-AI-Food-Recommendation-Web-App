using SharedLibrary.Common.Messaging;
using SharedLibrary.Common.ResponseModel;
using AutoMapper;
using Domain.Repositories;

namespace Application.RestaurantRatings.Queries;

public sealed record GetRatingByUserIdQuery(Guid UserId) : IQuery<IEnumerable<GetRatingResponse>>;

internal sealed class GetRatingByUserIdQueryHandler : IQueryHandler<GetRatingByUserIdQuery, IEnumerable<GetRatingResponse>>
{
    private readonly IRestaurantRatingRepository _restaurantRatingRepository;
    private readonly IMapper _mapper;

    public GetRatingByUserIdQueryHandler(IRestaurantRatingRepository restaurantRatingRepository, IMapper mapper)
    {
        _restaurantRatingRepository = restaurantRatingRepository;
        _mapper = mapper;
    }
    public async Task<Result<IEnumerable<GetRatingResponse>>> Handle(GetRatingByUserIdQuery request, CancellationToken cancellationToken)
    {
        var rating = await _restaurantRatingRepository.GetByUserIdAsync(request.UserId, cancellationToken);

        var ratingResponses = _mapper.Map<IEnumerable<GetRatingResponse>>(rating);
        return Result.Success(ratingResponses);
    }
}
using SharedLibrary.Common.Messaging;
using SharedLibrary.Common.ResponseModel;
using AutoMapper;
using Domain.Repositories;

namespace Application.RestaurantRatings.Queries;

public sealed record GetAllRatingsQuery : IQuery<IEnumerable<GetRatingResponse>>;
internal sealed class GetAllRatingsQueryHandler : IQueryHandler<GetAllRatingsQuery, IEnumerable<GetRatingResponse>>
{
    private readonly IRestaurantRatingRepository _restaurantRatingRepository;
    private readonly IMapper _mapper;

    public GetAllRatingsQueryHandler(IRestaurantRatingRepository restaurantRatingRepository, IMapper mapper)
    {
        _restaurantRatingRepository = restaurantRatingRepository;
        _mapper = mapper;
    }

    public async Task<Result<IEnumerable<GetRatingResponse>>> Handle(GetAllRatingsQuery request, CancellationToken cancellationToken)
    {
        var ratings = await _restaurantRatingRepository.GetAllAsync(cancellationToken);
        var ratingResponses = _mapper.Map<IEnumerable<GetRatingResponse>>(ratings);
        return Result.Success(ratingResponses);
    }   
}

using SharedLibrary.Common.Messaging;
using SharedLibrary.Common.ResponseModel;
using AutoMapper;
using Domain.Repositories;

namespace Application.RestaurantRatings.Queries;

public sealed record GetRatingByIdQuery(Guid Id) : IQuery<IEnumerable<GetRatingResponse>>;

internal sealed class GetRatingByIdQueryHandler : IQueryHandler<GetRatingByIdQuery, IEnumerable<GetRatingResponse>>
{
    private readonly IRestaurantRatingRepository _restaurantRatingRepository;
    private readonly IMapper _mapper;

    public GetRatingByIdQueryHandler(IRestaurantRatingRepository restaurantRatingRepository, IMapper mapper)
    {
        _restaurantRatingRepository = restaurantRatingRepository;
        _mapper = mapper;
    }
    public async Task<Result<IEnumerable<GetRatingResponse>>> Handle(GetRatingByIdQuery request, CancellationToken cancellationToken)
    {
        var rating = await _restaurantRatingRepository.GetByIdAsync(request.Id, cancellationToken);

        var ratingResponse = _mapper.Map<IEnumerable<GetRatingResponse>>(rating);
        return Result.Success(ratingResponse);
    }
}
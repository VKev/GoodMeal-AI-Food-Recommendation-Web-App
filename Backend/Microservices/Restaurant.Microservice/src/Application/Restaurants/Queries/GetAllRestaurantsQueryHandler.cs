using SharedLibrary.Common.Messaging;
using SharedLibrary.Common.ResponseModel;
using AutoMapper;
using Domain.Repositories;

namespace Application.Restaurants.Queries;

public sealed record GetAllRestaurantsQuery : IQuery<IEnumerable<GetRestaurantResponse>>;
internal sealed class GetAllRestaurantsQueryHandler : IQueryHandler<GetAllRestaurantsQuery, IEnumerable<GetRestaurantResponse>>
{
    private readonly IRestaurantRepository _restaurantRepository;
    private readonly IMapper _mapper;

    public GetAllRestaurantsQueryHandler(IRestaurantRepository restaurantRepository, IMapper mapper)
    {
        _restaurantRepository = restaurantRepository;
        _mapper = mapper;
    }

    public async Task<Result<IEnumerable<GetRestaurantResponse>>> Handle(GetAllRestaurantsQuery request, CancellationToken cancellationToken)
    {
        var restaurants = await _restaurantRepository.GetAllAsync(cancellationToken);
        var restaurantResponses = _mapper.Map<IEnumerable<GetRestaurantResponse>>(restaurants);
        return Result.Success(restaurantResponses);
    }
}
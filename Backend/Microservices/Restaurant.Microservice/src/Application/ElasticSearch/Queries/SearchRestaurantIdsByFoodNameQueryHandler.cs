using AutoMapper;
using Domain.Repositories;
using SharedLibrary.Common.Messaging;
using SharedLibrary.Common.ResponseModel;

namespace Application.Restaurants.Queries;

public sealed record SearchRestaurantByFoodNameQuery(string Keyword) : IQuery<IEnumerable<GetRestaurantResponse>>;

internal sealed record 
    SearchRestaurantByFoodNameQueryHandler : IQueryHandler<SearchRestaurantByFoodNameQuery,
    IEnumerable<GetRestaurantResponse>>
{
    private readonly IFoodElasticRepository _foodElasticRepository;
    private readonly IRestaurantRepository _restaurantRepository;
    private readonly IMapper _mapper;

    public SearchRestaurantByFoodNameQueryHandler(IFoodElasticRepository foodElasticRepository,
        IRestaurantRepository restaurantRepository, IMapper mapper)
    {
        _foodElasticRepository = foodElasticRepository;
        _restaurantRepository = restaurantRepository;
        _mapper = mapper;
    }

    public async Task<Result<IEnumerable<GetRestaurantResponse>>> Handle(SearchRestaurantByFoodNameQuery request,
        CancellationToken cancellationToken)
    {
        var restaurantResponses = new List<GetRestaurantResponse>();
        var restaurantIds =
            await _foodElasticRepository.SearchRestaurantIdsByFoodNameAsync(request.Keyword, cancellationToken);
        foreach (var restaurantId in restaurantIds)
        {
            var restaurant = await _restaurantRepository.GetByIdAsync(restaurantId, cancellationToken);
            var restaurantResponse = _mapper.Map<GetRestaurantResponse>(restaurant);
            restaurantResponses.Add(restaurantResponse);
        }
        return Result.Success(restaurantResponses.AsEnumerable());
    }
}
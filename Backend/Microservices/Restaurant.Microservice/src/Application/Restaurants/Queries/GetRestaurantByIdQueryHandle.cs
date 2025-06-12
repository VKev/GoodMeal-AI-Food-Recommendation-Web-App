using SharedLibrary.Common.Messaging;
using SharedLibrary.Common.ResponseModel;
using AutoMapper;
using Domain.Repositories;

namespace Application.Restaurants.Queries;

public sealed record GetRestaurantByIdQuery(Guid Id) : IQuery<IEnumerable<GetRestaurantResponse>>;

internal sealed class GetRestaurantByIdQueryHandler : IQueryHandler<GetRestaurantByIdQuery, IEnumerable<GetRestaurantResponse>>
{
    private readonly IRestaurantRepository _restaurantRepository;
    private readonly IMapper _mapper;

    public GetRestaurantByIdQueryHandler(IRestaurantRepository restaurantRepository, IMapper mapper)
    {
        _restaurantRepository = restaurantRepository;
        _mapper = mapper;
    }
    public async Task<Result<IEnumerable<GetRestaurantResponse>>> Handle(GetRestaurantByIdQuery request, CancellationToken cancellationToken)
    {
        var restaurant = await _restaurantRepository.GetByIdAsync(request.Id, cancellationToken);

        var restaurantResponse = _mapper.Map<IEnumerable<GetRestaurantResponse>>(restaurant);
        return Result.Success(restaurantResponse);
    }
}

using Application.RestaurantRatings.Queries;
using AutoMapper;
using Domain.Repositories;
using SharedLibrary.Common.Messaging;
using SharedLibrary.Common.ResponseModel;

namespace Application.Foods.Queries;

public sealed record GetFoodByRestaurantIdQuery(Guid RestaurantId) : IQuery<IEnumerable<GetFoodResponse>>;

internal sealed class GetFoodByRestaurantIdHandler : IQueryHandler<GetFoodByRestaurantIdQuery, IEnumerable<GetFoodResponse>>
{
    private readonly IFoodRepository _foodRepository;
    private readonly IMapper _mapper;

    public GetFoodByRestaurantIdHandler(IMapper mapper, IFoodRepository foodRepository)
    {
        _mapper = mapper;
        _foodRepository = foodRepository;
    }
    public async Task<Result<IEnumerable<GetFoodResponse>>> Handle(GetFoodByRestaurantIdQuery request, CancellationToken cancellationToken)
    {
        var food = await _foodRepository.GetByRestaurantIdAsync(request.RestaurantId, cancellationToken);

        var foodResponses = _mapper.Map<IEnumerable<GetFoodResponse>>(food);
        return Result.Success(foodResponses);
    }
}
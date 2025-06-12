using SharedLibrary.Common.Messaging;
using SharedLibrary.Common.ResponseModel;
using AutoMapper;
using Domain.Repositories;

namespace Application.Foods.Queries;

public sealed record GetAllFoodsQuery : IQuery<IEnumerable<GetFoodResponse>>;
internal sealed class GetAllFoodsQueryHandler : IQueryHandler<GetAllFoodsQuery, IEnumerable<GetFoodResponse>>
{
    private readonly IFoodRepository _foodRepository;
    private readonly IMapper _mapper;

    public GetAllFoodsQueryHandler(IFoodRepository foodRepository, IMapper mapper)
    {
        _foodRepository = foodRepository;
        _mapper = mapper;
    }

    public async Task<Result<IEnumerable<GetFoodResponse>>> Handle(GetAllFoodsQuery request, CancellationToken cancellationToken)
    {
        var foods = await _foodRepository.GetAllAsync(cancellationToken);
        var foodResponses = _mapper.Map<IEnumerable<GetFoodResponse>>(foods);
        return Result.Success(foodResponses);
    }   
}

using Application.Abstractions.Messaging;
using Application.Common.ResponseModel;
using AutoMapper;
using Domain.Repositories;

namespace Application.Foods.Queries;

public sealed record GetFoodByIdQuery(Guid Id) : IQuery<IEnumerable<GetFoodResponse>>;

internal sealed class GetFoodByIdQueryHandler : IQueryHandler<GetFoodByIdQuery, IEnumerable<GetFoodResponse>>
{
    private readonly IFoodRepository _foodRepository;
    private readonly IMapper _mapper;

    public GetFoodByIdQueryHandler(IFoodRepository foodRepository, IMapper mapper)
    {
        _foodRepository = foodRepository;
        _mapper = mapper;
    }
    public async Task<Result<IEnumerable<GetFoodResponse>>> Handle(GetFoodByIdQuery request, CancellationToken cancellationToken)
    {
        var food = await _foodRepository.GetByIdAsync(request.Id, cancellationToken);

        var foodResponse = _mapper.Map<IEnumerable<GetFoodResponse>>(food);
        return Result.Success(foodResponse);
    }
}
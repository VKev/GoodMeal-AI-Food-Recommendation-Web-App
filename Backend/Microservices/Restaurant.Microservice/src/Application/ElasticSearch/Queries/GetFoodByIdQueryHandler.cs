using AutoMapper;
using Domain.Repositories;
using SharedLibrary.Common.Messaging;
using SharedLibrary.Common.ResponseModel;

namespace Application.ElasticSearch.Queries;

public sealed record GetFoodByIdQuery(string Id) : IQuery<GetFoodResponse>;
internal sealed record GetFoodByIdQueryHandler: IQueryHandler<GetFoodByIdQuery, GetFoodResponse>
{
    private readonly IFoodElasticRepository  _foodElasticRepository;
    private readonly IMapper _mapper;

    public GetFoodByIdQueryHandler(IFoodElasticRepository foodElasticRepository, IMapper mapper)
    {
        _foodElasticRepository = foodElasticRepository;
        _mapper = mapper;
    }

    public async Task<Result<GetFoodResponse>> Handle(GetFoodByIdQuery request, CancellationToken cancellationToken)
    {
        var food = await _foodElasticRepository.Get(request.Id, cancellationToken);
        var response = _mapper.Map<GetFoodResponse>(food);
        return Result.Success(response);
    }
}
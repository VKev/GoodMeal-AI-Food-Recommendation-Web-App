using AutoMapper;
using Domain.Repositories;
using SharedLibrary.Common.Messaging;
using SharedLibrary.Common.ResponseModel;

namespace Application.ElasticSearch.Queries;

public sealed record GetAllFoodQuery : IQuery<IEnumerable<GetFoodResponse>>;
public class GetAllFoodQueryHandler: IQueryHandler<GetAllFoodQuery, IEnumerable<GetFoodResponse>>
{
    private readonly IFoodElasticRepository _foodElasticRepository;
    private readonly IMapper _mapper;
    public GetAllFoodQueryHandler(IFoodElasticRepository foodElasticRepository, IMapper mapper)
    {
        _foodElasticRepository = foodElasticRepository;
        _mapper = mapper;
    }

    public async Task<Result<IEnumerable<GetFoodResponse>>> Handle(GetAllFoodQuery request, CancellationToken cancellationToken)
    {
        var foods = await _foodElasticRepository.GetAll(cancellationToken);
        var response = _mapper.Map<IEnumerable<GetFoodResponse>>(foods);
        return Result.Success(response);
    }
}
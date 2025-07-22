using Domain.Entities;
using Domain.Repositories;
using SharedLibrary.Common.Messaging;
using SharedLibrary.Common.ResponseModel;

namespace Application.ElasticSearch.Commands;

public sealed record AddOrUpdateFoodBulkCommand : ICommand;
internal sealed record AddOrUpdateBulkFoodCommandHandler: ICommandHandler<AddOrUpdateFoodBulkCommand>
{
    private readonly IFoodElasticRepository _foodElasticRepository;
    private readonly IFoodRepository _foodRepository;

    public AddOrUpdateBulkFoodCommandHandler(IFoodElasticRepository foodElasticRepository, IFoodRepository foodRepository)
    {
        _foodElasticRepository = foodElasticRepository;
        _foodRepository = foodRepository;
    }

    public async Task<Result> Handle(AddOrUpdateFoodBulkCommand request, CancellationToken cancellationToken)
    {
        var foods = await _foodRepository.GetAllAsync(cancellationToken);
        await _foodElasticRepository.AddOrUpdateBulk(foods, cancellationToken);
        return Result.Success();
    }
}
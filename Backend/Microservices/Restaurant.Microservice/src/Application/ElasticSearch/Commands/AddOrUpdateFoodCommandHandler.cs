using Domain.Entities;
using Domain.Repositories;
using SharedLibrary.Common.Messaging;
using SharedLibrary.Common.ResponseModel;

namespace Application.ElasticSearch.Commands;

public sealed record AddOrUpdateFoodCommand(Food Food): ICommand;
internal sealed record AddOrUpdateFoodCommandHandler : ICommandHandler<AddOrUpdateFoodCommand>
{
    private readonly IFoodElasticRepository _foodElasticRepository;

    public AddOrUpdateFoodCommandHandler(IFoodElasticRepository foodElasticRepository)
    {
        _foodElasticRepository = foodElasticRepository;
    }

    public async Task<Result> Handle(AddOrUpdateFoodCommand request, CancellationToken cancellationToken)
    {
        await _foodElasticRepository.AddOrUpdate(request.Food, cancellationToken);
        return Result.Success();
    }
}
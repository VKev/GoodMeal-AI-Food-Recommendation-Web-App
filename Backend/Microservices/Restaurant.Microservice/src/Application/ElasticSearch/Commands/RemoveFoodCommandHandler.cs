using Domain.Repositories;
using SharedLibrary.Common.Messaging;
using SharedLibrary.Common.ResponseModel;

namespace Application.ElasticSearch.Commands;

public sealed record RemoveFoodCommand(string FoodId) : ICommand;
internal sealed record RemoveFoodCommandHandler: ICommandHandler<RemoveFoodCommand>
{
    private readonly IFoodElasticRepository _foodElasticRepository;

    public RemoveFoodCommandHandler(IFoodElasticRepository foodElasticRepository)
    {
        _foodElasticRepository = foodElasticRepository;
    }

    public async Task<Result> Handle(RemoveFoodCommand request, CancellationToken cancellationToken)
    {
        await _foodElasticRepository.Remove(request.FoodId,  cancellationToken);
        return Result.Success();
    }
}
using Domain.Repositories;
using SharedLibrary.Common.Messaging;
using SharedLibrary.Common.ResponseModel;

namespace Application.Restaurants.Commands;

public sealed record CreateFoodIndexCommand(string indexName) : ICommand;
internal sealed record CreateFoodIndexCommandHandler:  ICommandHandler<CreateFoodIndexCommand>
{
    private readonly IFoodElasticRepository  _foodElasticRepository;

    public CreateFoodIndexCommandHandler(IFoodElasticRepository foodElasticRepository)
    {
        _foodElasticRepository = foodElasticRepository;
    }

    public async Task<Result> Handle(CreateFoodIndexCommand request, CancellationToken cancellationToken)
    {
        await _foodElasticRepository.CreateIndexIfNotExistsAsync(request.indexName, cancellationToken);
        return Result.Success();
    }
}
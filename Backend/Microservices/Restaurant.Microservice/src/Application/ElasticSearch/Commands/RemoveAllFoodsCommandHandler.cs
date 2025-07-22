using Domain.Repositories;
using SharedLibrary.Common.Messaging;
using SharedLibrary.Common.ResponseModel;

namespace Application.ElasticSearch.Commands;

public sealed record RemoveAllFoodsCommand() : ICommand;
internal sealed record RemoveAllFoodsCommandHandler: ICommandHandler<RemoveAllFoodsCommand>
{
    private readonly IFoodElasticRepository  _foodElasticRepository;

    public RemoveAllFoodsCommandHandler(IFoodElasticRepository foodElasticRepository)
    {
        _foodElasticRepository = foodElasticRepository;
    }

    public async Task<Result> Handle(RemoveAllFoodsCommand request, CancellationToken cancellationToken)
    {
        await _foodElasticRepository.RemoveAll(cancellationToken);
        return Result.Success();
    }
}
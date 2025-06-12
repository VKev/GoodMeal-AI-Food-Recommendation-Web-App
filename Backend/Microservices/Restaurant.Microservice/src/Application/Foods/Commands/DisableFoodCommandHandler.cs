using SharedLibrary.Common.Messaging;
using SharedLibrary.Common.ResponseModel;
using Domain.Repositories;
using SharedLibrary.Common;
namespace Application.Foods.Commands;

public sealed record DisableFoodCommand(
    Guid Id
) : ICommand;
internal sealed class DisableFoodCommandHandler : ICommandHandler<DisableFoodCommand>
{
    private readonly IFoodRepository _foodRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DisableFoodCommandHandler(IFoodRepository foodRepository, IUnitOfWork unitOfWork)
    {
        _foodRepository = foodRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DisableFoodCommand command, CancellationToken cancellationToken)
    {
        var food = await _foodRepository.GetByIdAsync(command.Id, cancellationToken);

        food.IsDisable = true;
        food.DisableAt = DateTime.UtcNow;
        food.UpdatedAt = DateTime.UtcNow;

        _foodRepository.Update(food);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
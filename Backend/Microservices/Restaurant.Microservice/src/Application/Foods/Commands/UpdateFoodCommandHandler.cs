using SharedLibrary.Common.Messaging;
using SharedLibrary.Common.ResponseModel;
using AutoMapper;
using Domain.Repositories;
using SharedLibrary.Common;
namespace Application.Foods.Commands;

public sealed record UpdateFoodCommand(
    Guid Id,
    string? Name,
    string? Description,
    decimal? Price,
    bool? IsAvailable
) : ICommand;

internal sealed class UpdateFoodCommandHandler : ICommandHandler<UpdateFoodCommand>
{
    private readonly IFoodRepository _foodRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateFoodCommandHandler(IFoodRepository foodRepository, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _foodRepository = foodRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result> Handle(UpdateFoodCommand command, CancellationToken cancellationToken)
    {
        var food = await _foodRepository.GetByIdAsync(command.Id, cancellationToken);

        _mapper.Map(command, food);
        food.UpdatedAt = DateTime.UtcNow;

        _foodRepository.Update(food);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
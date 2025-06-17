using SharedLibrary.Common.Messaging;
using SharedLibrary.Common.ResponseModel;
using AutoMapper;
using Domain.Entities;
using Domain.Repositories;
using SharedLibrary.Common;
namespace Application.Foods.Commands;

public sealed record CreateFoodCommand(
    string Name,
    string Description,
    decimal Price,
    bool? IsAvailable,
    Guid RestaurantId
) : ICommand;

internal sealed class CreateFoodCommandHandler : ICommandHandler<CreateFoodCommand>
{
    private readonly IFoodRepository _foodRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateFoodCommandHandler(IFoodRepository foodRepository, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _foodRepository = foodRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    public async Task<Result> Handle(CreateFoodCommand command, CancellationToken cancellationToken)
    {
        var food = _mapper.Map<Food>(command);
        food.CreatedAt = DateTime.UtcNow;
        food.IsDisable = false;
        await _foodRepository.AddAsync(food, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

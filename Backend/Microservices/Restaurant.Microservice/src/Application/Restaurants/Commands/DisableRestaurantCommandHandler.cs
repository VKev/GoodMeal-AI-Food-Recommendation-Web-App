using Application.Abstractions.Messaging;
using Application.Abstractions.UnitOfWork;
using Application.Common.ResponseModel;
using Domain.Repositories;

namespace Application.Restaurants.Commands;

public sealed record DisableRestaurantCommand(
    Guid Id
) : ICommand;

internal sealed class DisableRestaurantCommandHandler : ICommandHandler<DisableRestaurantCommand>
{
    private readonly IRestaurantRepository _restaurantRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DisableRestaurantCommandHandler(IRestaurantRepository restaurantRepository, IUnitOfWork unitOfWork)
    {
        _restaurantRepository = restaurantRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DisableRestaurantCommand command, CancellationToken cancellationToken)
    {
        var restaurant = await _restaurantRepository.GetByIdAsync(command.Id, cancellationToken);

        restaurant.IsDisable = true;
        restaurant.UpdatedAt = DateTime.UtcNow;

        _restaurantRepository.Update(restaurant);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
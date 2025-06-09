using Application.Abstractions.Messaging;
using Application.Abstractions.UnitOfWork;
using Application.Common.ResponseModel;
using Domain.Repositories;

namespace Application.RestaurantRatings.Commands;

public sealed record DisableRatingCommand(
    Guid Id
) : ICommand;

internal sealed class DisableRatingCommandHandler : ICommandHandler<DisableRatingCommand>
{
    private readonly IRestaurantRatingRepository _restaurantRatingRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DisableRatingCommandHandler(IRestaurantRatingRepository restaurantRatingRepository, IUnitOfWork unitOfWork)
    {
        _restaurantRatingRepository = restaurantRatingRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DisableRatingCommand command, CancellationToken cancellationToken)
    {
        var rating = await _restaurantRatingRepository.GetByIdAsync(command.Id, cancellationToken);

        rating.IsDisable = true;
        rating.DisableAt = DateTime.UtcNow;
        rating.UpdatedAt = DateTime.UtcNow;

        _restaurantRatingRepository.Update(rating);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
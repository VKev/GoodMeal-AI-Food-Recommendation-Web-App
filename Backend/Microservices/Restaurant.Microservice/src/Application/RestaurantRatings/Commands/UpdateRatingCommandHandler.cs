using Application.Abstractions.Messaging;
using Application.Abstractions.UnitOfWork;
using Application.Common.ResponseModel;
using AutoMapper;
using Domain.Repositories;

namespace Application.RestaurantRatings.Commands;

public sealed record UpdateRatingCommand(
    Guid Id,
    int Rating,
    string? Comment
) : ICommand;

internal sealed class UpdateFoodCommandHandler : ICommandHandler<UpdateRatingCommand>
{
    private readonly IRestaurantRatingRepository _restaurantRatingRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateFoodCommandHandler(IRestaurantRatingRepository restaurantRatingRepository, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _restaurantRatingRepository = restaurantRatingRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result> Handle(UpdateRatingCommand command, CancellationToken cancellationToken)
    {
        var restaurantRating = await _restaurantRatingRepository.GetByIdAsync(command.Id, cancellationToken);

        _mapper.Map(command, restaurantRating);
        restaurantRating.UpdatedAt = DateTime.UtcNow;

        _restaurantRatingRepository.Update(restaurantRating);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
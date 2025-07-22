using SharedLibrary.Common.Messaging;
using SharedLibrary.Common.ResponseModel;
using AutoMapper;
using Domain.Repositories;
using SharedLibrary.Common;
namespace Application.RestaurantRatings.Commands;

public sealed record UpdateRatingCommand(
    Guid Id,
    string? Comment,
    string? ImageUrl
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

        if (!string.IsNullOrWhiteSpace(command.Comment))
            restaurantRating.Comment = command.Comment;

        if (!string.IsNullOrWhiteSpace(command.ImageUrl))
            restaurantRating.ImageUrl = command.ImageUrl;

        restaurantRating.UpdatedAt = DateTime.UtcNow;

        _restaurantRatingRepository.Update(restaurantRating);
        // await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
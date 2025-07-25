using AutoMapper;
using Domain.Repositories;
using Microsoft.Extensions.Logging;
using SharedLibrary.Common;
using SharedLibrary.Common.Messaging;
using SharedLibrary.Common.ResponseModel;

namespace Application.RestaurantRatings.Commands;

public sealed record UpdateRatingScoreCommand(
    Guid Id,
    float Rating
) : ICommand;

internal sealed class UpdateRatingScoreCommandHandler : ICommandHandler<UpdateRatingScoreCommand>
{
    private readonly IRestaurantRatingRepository _restaurantRatingRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<UpdateRatingScoreCommandHandler> _logger;
    

    public UpdateRatingScoreCommandHandler(IRestaurantRatingRepository restaurantRatingRepository, IUnitOfWork unitOfWork, IMapper mapper, ILogger<UpdateRatingScoreCommandHandler> logger)
    {
        _restaurantRatingRepository = restaurantRatingRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result> Handle(UpdateRatingScoreCommand command, CancellationToken cancellationToken)
    {
        var restaurantRating = await _restaurantRatingRepository.GetByIdAsync(command.Id, cancellationToken);

        if (command.Rating != null)
        {
            restaurantRating.Rating = command.Rating;
            _logger.LogInformation("Updated Rating Score: {RatingScore} for Rating Id: {RatingId}", command.Rating, command.Id);
        }
        

        restaurantRating.UpdatedAt = DateTime.UtcNow;

        _restaurantRatingRepository.Update(restaurantRating);
        // await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
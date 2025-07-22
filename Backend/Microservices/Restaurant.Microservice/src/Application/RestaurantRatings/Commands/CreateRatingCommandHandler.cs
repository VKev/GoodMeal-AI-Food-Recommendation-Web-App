using SharedLibrary.Common.Messaging;
using SharedLibrary.Common.ResponseModel;
using AutoMapper;
using Domain.Entities;
using Domain.Repositories;
using MassTransit;
using SharedLibrary.Common;
using SharedLibrary.Contracts.RatingPrompt;

namespace Application.RestaurantRatings.Commands;

public sealed record CreateRatingCommand(
    Guid RestaurantId,
    Guid UserId,
    string Comment,
    string? ImageUrl
) : ICommand;

internal sealed class CreateFoodCommandHandler : ICommandHandler<CreateRatingCommand>
{
    private readonly IRestaurantRatingRepository _restaurantRatingRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IPublishEndpoint _publishEndpoint;

    public CreateFoodCommandHandler(IRestaurantRatingRepository restaurantRatingRepository, IUnitOfWork unitOfWork, IMapper mapper, IPublishEndpoint publishEndpoint)
    {
        _restaurantRatingRepository = restaurantRatingRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _publishEndpoint = publishEndpoint;
    }
    public async Task<Result> Handle(CreateRatingCommand command, CancellationToken cancellationToken)
    {
        var restaurantRating = _mapper.Map<RestaurantRating>(command);
        restaurantRating.CreatedAt = DateTime.UtcNow;
        await _restaurantRatingRepository.AddAsync(restaurantRating, cancellationToken);
        var correlationId = Guid.NewGuid();
        await _publishEndpoint.Publish(new RatingCreatedSagaStart
        {
            CorrelationId = correlationId,
            RatingId = restaurantRating.Id,
            UserId = restaurantRating.UserId,
            RestaurantId = restaurantRating.RestaurantId,
            Comment = restaurantRating.Comment,
            CreatedAt = restaurantRating.CreatedAt
        }, cancellationToken);
        // await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
    
}

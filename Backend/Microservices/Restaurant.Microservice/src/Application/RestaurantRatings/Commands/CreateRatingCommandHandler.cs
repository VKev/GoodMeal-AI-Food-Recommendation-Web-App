using SharedLibrary.Common.Messaging;
using SharedLibrary.Common.ResponseModel;
using AutoMapper;
using Domain.Entities;
using Domain.Repositories;
using SharedLibrary.Common;

namespace Application.RestaurantRatings.Commands;

public sealed record CreateRatingCommand(
    Guid RestaurantId,
    Guid UserId,
    string? Comment
) : ICommand;

internal sealed class CreateFoodCommandHandler : ICommandHandler<CreateRatingCommand>
{
    private readonly IRestaurantRatingRepository _restaurantRatingRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateFoodCommandHandler(IRestaurantRatingRepository restaurantRatingRepository, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _restaurantRatingRepository = restaurantRatingRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    public async Task<Result> Handle(CreateRatingCommand command, CancellationToken cancellationToken)
    {
        var restaurantRating = _mapper.Map<RestaurantRating>(command);
        restaurantRating.CreatedAt = DateTime.UtcNow;
        await _restaurantRatingRepository.AddAsync(restaurantRating, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
    
}

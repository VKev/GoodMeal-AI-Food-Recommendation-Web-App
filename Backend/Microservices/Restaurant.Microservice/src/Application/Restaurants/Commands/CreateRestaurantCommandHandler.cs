using SharedLibrary.Common.Messaging;
using SharedLibrary.Common.ResponseModel;
using AutoMapper;
using Domain.Entities;
using Domain.Repositories;
using SharedLibrary.Common;
namespace Application.Restaurants.Commands;

//Create DTO
public sealed record CreateRestaurantCommand(
    string Name,
    string Address,
    string Phone,
    string? PlaceLink,
    string? Website,
    string? Types,
    float Latitude,
    float Longitude,
    string? TimeZone,
    string? Description,
    string? ImageUrl
) : ICommand;

internal sealed class CreateRestaurantCommandHandler : ICommandHandler<CreateRestaurantCommand>
{
    private readonly IRestaurantRepository _restaurantRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateRestaurantCommandHandler(IRestaurantRepository restaurantRepository, IMapper mapper,
        IUnitOfWork unitOfWork)
    {
        _restaurantRepository = restaurantRepository;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }


    public async Task<Result> Handle(CreateRestaurantCommand command, CancellationToken cancellationToken)
    {
        var restaurant = _mapper.Map<Restaurant>(command);
        restaurant.CreatedAt = DateTime.UtcNow;
        restaurant.IsDisable = false;
        await _restaurantRepository.AddAsync(restaurant, cancellationToken);
        // await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
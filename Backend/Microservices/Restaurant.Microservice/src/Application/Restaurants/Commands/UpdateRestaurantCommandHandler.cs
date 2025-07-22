using SharedLibrary.Common.Messaging;
using SharedLibrary.Common.ResponseModel;
using AutoMapper;
using Domain.Repositories;
using SharedLibrary.Common;

namespace Application.Restaurants.Commands;

public sealed record UpdateRestaurantCommand(
    Guid Id,
    string? Name,
    string? Address,
    string? Phone,
    string? PlaceLink,
    string? Website,
    string? Types,
    float? Latitude,
    float? Longitude,
    string? TimeZone,
    string? Description,
    string? ImageUrl
) : ICommand;

internal sealed record UpdateRestaurantCommandHandler : ICommandHandler<UpdateRestaurantCommand>
{
    private readonly IRestaurantRepository _restaurantRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateRestaurantCommandHandler(IRestaurantRepository restaurantRepository, IMapper mapper,
        IUnitOfWork unitOfWork)
    {
        _restaurantRepository = restaurantRepository;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateRestaurantCommand command, CancellationToken cancellationToken)
    {
        var restaurant = await _restaurantRepository.GetByIdAsync(command.Id, cancellationToken);

        if (!string.IsNullOrWhiteSpace(command.Name))
            restaurant.Name = command.Name;

        if (!string.IsNullOrWhiteSpace(command.Address))
            restaurant.Address = command.Address;

        if (!string.IsNullOrWhiteSpace(command.Phone))
            restaurant.Phone = command.Phone;

        if (!string.IsNullOrWhiteSpace(command.PlaceLink))
            restaurant.PlaceLink = command.PlaceLink;

        if (!string.IsNullOrWhiteSpace(command.Website))
            restaurant.Website = command.Website;

        if (!string.IsNullOrWhiteSpace(command.Types))
            restaurant.Types = command.Types;

        if (command.Latitude.HasValue)
            restaurant.Latitude = command.Latitude.Value;

        if (command.Longitude.HasValue)
            restaurant.Longitude = command.Longitude.Value;

        if (!string.IsNullOrWhiteSpace(command.TimeZone))
            restaurant.TimeZone = command.TimeZone;

        if (!string.IsNullOrWhiteSpace(command.Description))
            restaurant.Description = command.Description;

        if (!string.IsNullOrWhiteSpace(command.ImageUrl))
            restaurant.ImageUrl = command.ImageUrl;
        restaurant.UpdatedAt = DateTime.UtcNow;

        _restaurantRepository.Update(restaurant);
        // await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
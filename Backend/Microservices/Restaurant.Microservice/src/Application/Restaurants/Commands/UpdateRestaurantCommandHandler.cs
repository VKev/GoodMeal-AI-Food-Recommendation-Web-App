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
    string? Phone
) : ICommand;

internal sealed record UpdateRestaurantCommandHandler : ICommandHandler<UpdateRestaurantCommand>
{
    private readonly IRestaurantRepository _restaurantRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateRestaurantCommandHandler(IRestaurantRepository restaurantRepository, IMapper mapper, IUnitOfWork unitOfWork)
    {
        _restaurantRepository = restaurantRepository;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateRestaurantCommand command, CancellationToken cancellationToken)
    {
        var restaurant = await _restaurantRepository.GetByIdAsync(command.Id, cancellationToken);

        _mapper.Map(command, restaurant);
        restaurant.UpdatedAt = DateTime.UtcNow;

        _restaurantRepository.Update(restaurant);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
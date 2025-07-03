using System.Security.Claims;
using FluentValidation;
using Microsoft.Extensions.Logging;
using SharedLibrary.Common.Messaging;
using SharedLibrary.Common.ResponseModel;
using Domain.Repositories;
using Microsoft.AspNetCore.Http;
using SharedLibrary.Common.Event;
using SharedLibrary.Contracts.RestaurantCreating;

namespace Application.Business.Commands.AddRestaurantToBusinessCommand;

public sealed record AddRestaurantToBusinessCommand(
    Guid BusinessId,
    string Name,
    string? Address,
    string? Phone
) : ICommand<AddRestaurantToBusinessResponse>;

public sealed class AddRestaurantToBusinessResponse
{
    public Guid CorrelationId { get; set; }
    public Guid BusinessId { get; set; }
    public Guid RestaurantId { get; set; }
    public string Name { get; set; }
    public string Message { get; set; }

    public AddRestaurantToBusinessResponse()
    {
    }

    public AddRestaurantToBusinessResponse(Guid correlationId, Guid businessId, Guid restaurantId, string name,
        string message)
    {
        CorrelationId = correlationId;
        BusinessId = businessId;
        RestaurantId = restaurantId;
        Name = name;
        Message = message;
    }
}

internal sealed class AddRestaurantToBusinessCommandHandler : ICommandHandler<AddRestaurantToBusinessCommand, AddRestaurantToBusinessResponse>
{
    private readonly ILogger<AddRestaurantToBusinessCommandHandler> _logger;
    private readonly IBusinessRepository _businessRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IEventUnitOfWork _events;

    public AddRestaurantToBusinessCommandHandler(
        ILogger<AddRestaurantToBusinessCommandHandler> logger,
        IBusinessRepository businessRepository,
        IHttpContextAccessor httpContextAccessor,
        IEventUnitOfWork events)
    {
        _logger = logger;
        _businessRepository = businessRepository;
        _httpContextAccessor = httpContextAccessor;
        _events = events;
    }

    public async Task<Result<AddRestaurantToBusinessResponse>> Handle(AddRestaurantToBusinessCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim is null)
            {
                _logger.LogWarning("User ID not found in claims");
                return Result.Failure<AddRestaurantToBusinessResponse>(new Error("Auth.Unauthorized",
                    "User not authenticated"));
            }

            var userId = userIdClaim.Value;
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("User context is missing for restaurant creation");
                return Result.Failure<AddRestaurantToBusinessResponse>(new Error("Authorization.Failed",
                    "User context is required"));
            }

            var business = await _businessRepository.GetByIdAsync(request.BusinessId);
            if (business == null)
            {
                _logger.LogWarning("Business with ID {BusinessId} not found", request.BusinessId);
                return Result.Failure<AddRestaurantToBusinessResponse>(new Error("Business.NotFound", "Business not found"));
            }

            if (business.OwnerId != userId)
            {
                _logger.LogWarning("User {UserId} is not authorized to add restaurant to business {BusinessId}", userId,
                    request.BusinessId);
                return Result.Failure<AddRestaurantToBusinessResponse>(new Error("Authorization.Failed",
                    "You are not authorized to add restaurant to this business"));
            }

            if (business.IsDisable == true)
            {
                _logger.LogWarning("Cannot add restaurant to disabled business {BusinessId}", request.BusinessId);
                return Result.Failure<AddRestaurantToBusinessResponse>(new Error("Business.Disabled",
                    "Cannot add restaurant to a disabled business"));
            }

            var restaurantId = Guid.NewGuid();
            var correlationId = Guid.NewGuid();

            _events.Add(new RestaurantCreatingSagaStart
            {
                CorrelationId = correlationId,
                BusinessId = request.BusinessId,
                RestaurantId = restaurantId,
                Name = request.Name,
                Address = request.Address,
                Phone = request.Phone,
                CreatedBy = userId
            });

            var response = new AddRestaurantToBusinessResponse(
                correlationId,
                request.BusinessId,
                restaurantId,
                request.Name,
                "Restaurant creation process started"
            );

            _logger.LogInformation("Started restaurant creation process for business {BusinessId} with CorrelationId {CorrelationId}", 
                request.BusinessId, correlationId);
            
            return Result.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error adding restaurant to business {BusinessId}", request.BusinessId);
            return Result.Failure<AddRestaurantToBusinessResponse>(new Error("InternalError", "An unexpected error occurred"));
        }
    }
}

public class AddRestaurantToBusinessCommandValidator : AbstractValidator<AddRestaurantToBusinessCommand>
{
    public AddRestaurantToBusinessCommandValidator()
    {
        RuleFor(x => x.BusinessId)
            .NotEmpty()
            .WithMessage("Business ID is required");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Restaurant name is required")
            .MaximumLength(200)
            .WithMessage("Restaurant name cannot exceed 200 characters");

        RuleFor(x => x.Address)
            .MaximumLength(500)
            .WithMessage("Address cannot exceed 500 characters");

        RuleFor(x => x.Phone)
            .MaximumLength(20)
            .WithMessage("Phone cannot exceed 20 characters");
    }
} 
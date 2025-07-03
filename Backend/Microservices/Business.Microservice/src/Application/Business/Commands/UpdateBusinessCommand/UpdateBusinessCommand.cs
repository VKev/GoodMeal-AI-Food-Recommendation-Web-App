using System.Security.Claims;
using FluentValidation;
using Microsoft.Extensions.Logging;
using SharedLibrary.Common.Messaging;
using SharedLibrary.Common.ResponseModel;
using Domain.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using SharedLibrary.Common;

namespace Application.Business.Commands.UpdateBusinessCommand;

public sealed record UpdateBusinessCommand(
    Guid BusinessId,
    string Name,
    string? Description,
    string? Address,
    string? Phone,
    string? Email,
    string? Website
) : ICommand<UpdateBusinessResponse>;

public sealed class UpdateBusinessResponse
{
    public Guid Id { get; set; }
    public string? OwnerId { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Website { get; set; }
    public bool IsActive { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public UpdateBusinessResponse()
    {
    }

    public UpdateBusinessResponse(
        Guid id,
        string? ownerId,
        string name,
        string? description,
        string? address,
        string? phone,
        string? email,
        string? website,
        bool isActive,
        DateTime? updatedAt)
    {
        Id = id;
        OwnerId = ownerId;
        Name = name;
        Description = description;
        Address = address;
        Phone = phone;
        Email = email;
        Website = website;
        IsActive = isActive;
        UpdatedAt = updatedAt;
    }
}

internal sealed class UpdateBusinessCommandHandler : ICommandHandler<UpdateBusinessCommand, UpdateBusinessResponse>
{
    private readonly ILogger<UpdateBusinessCommandHandler> _logger;
    private readonly IBusinessRepository _businessRepository;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UpdateBusinessCommandHandler(
        ILogger<UpdateBusinessCommandHandler> logger,
        IBusinessRepository businessRepository,
        IMapper mapper, IHttpContextAccessor httpContextAccessor)
    {
        _logger = logger;
        _businessRepository = businessRepository;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Result<UpdateBusinessResponse>> Handle(UpdateBusinessCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim is null)
            {
                _logger.LogWarning("User ID not found in claims");
                return Result.Failure<UpdateBusinessResponse>(new Error("Auth.Unauthorized", "User not authenticated"));
            }

            var userId = userIdClaim.Value;
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("User context is missing for business update");
                return Result.Failure<UpdateBusinessResponse>(new Error("Authorization.Failed",
                    "User context is required"));
            }

            var business = await _businessRepository.GetByIdAsync(request.BusinessId);
            if (business == null)
            {
                _logger.LogWarning("Business with ID {BusinessId} not found", request.BusinessId);
                return Result.Failure<UpdateBusinessResponse>(new Error("Business.NotFound", "Business not found"));
            }

            if (business.OwnerId != userId)
            {
                _logger.LogWarning("User {UserId} is not authorized to update business {BusinessId}", userId,
                    request.BusinessId);
                return Result.Failure<UpdateBusinessResponse>(new Error("Authorization.Failed",
                    "You are not authorized to update this business"));
            }

            if (business.IsDisable == true)
            {
                _logger.LogWarning("Cannot update disabled business {BusinessId}", request.BusinessId);
                return Result.Failure<UpdateBusinessResponse>(new Error("Business.Disabled",
                    "Cannot update a disabled business"));
            }

            business.Name = request.Name;
            business.Description = request.Description;
            business.Address = request.Address;
            business.Phone = request.Phone;
            business.Email = request.Email;
            business.Website = request.Website;
            business.UpdatedAt = DateTime.UtcNow;

            _businessRepository.Update(business);

            var response = _mapper.Map<UpdateBusinessResponse>(business);

            _logger.LogInformation("Successfully updated business {BusinessId} by user {UserId}", request.BusinessId,
                userId);
            return Result.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error updating business {BusinessId}", request.BusinessId);
            return Result.Failure<UpdateBusinessResponse>(new Error("InternalError", "An unexpected error occurred"));
        }
    }
}

public class UpdateBusinessCommandValidator : AbstractValidator<UpdateBusinessCommand>
{
    public UpdateBusinessCommandValidator()
    {
        RuleFor(x => x.BusinessId)
            .NotEmpty()
            .WithMessage("Business ID is required");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Business name is required")
            .MaximumLength(200)
            .WithMessage("Business name cannot exceed 200 characters");

        RuleFor(x => x.Description)
            .MaximumLength(1000)
            .WithMessage("Description cannot exceed 1000 characters")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.Address)
            .MaximumLength(500)
            .WithMessage("Address cannot exceed 500 characters")
            .When(x => !string.IsNullOrEmpty(x.Address));

        RuleFor(x => x.Phone)
            .Matches(@"^[\+]?[0-9\s\-\(\)]{8,20}$")
            .WithMessage("Invalid phone number format")
            .When(x => !string.IsNullOrEmpty(x.Phone));

        RuleFor(x => x.Email)
            .EmailAddress()
            .WithMessage("Invalid email format")
            .When(x => !string.IsNullOrEmpty(x.Email));

        RuleFor(x => x.Website)
            .Must(BeValidUrl)
            .WithMessage("Invalid website URL format")
            .When(x => !string.IsNullOrEmpty(x.Website));
    }

    private bool BeValidUrl(string? url)
    {
        if (string.IsNullOrEmpty(url))
            return true;

        return Uri.TryCreate(url, UriKind.Absolute, out var result) &&
               (result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps);
    }
}
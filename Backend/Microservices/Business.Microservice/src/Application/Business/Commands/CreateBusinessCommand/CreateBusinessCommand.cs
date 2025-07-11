using System.Security.Claims;
using FluentValidation;
using Microsoft.Extensions.Logging;
using SharedLibrary.Common.Messaging;
using SharedLibrary.Common.ResponseModel;
using Domain.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using SharedLibrary.Common;

namespace Application.Business.Commands.CreateBusinessCommand;

public sealed record CreateBusinessCommand(
    string Name,
    string? Description,
    string? Address,
    string? Phone,
    string? Email,
    string? Website,
    string CreateReason
) : ICommand<CreateBusinessResponse>;

public sealed class CreateBusinessResponse
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
    public DateTime? CreatedAt { get; set; }
    public string CreateReason { get; set; }

    public CreateBusinessResponse()
    {
    }

    public CreateBusinessResponse(
        Guid id,
        string? ownerId,
        string name,
        string? description,
        string? address,
        string? phone,
        string? email,
        string? website,
        bool isActive,
        DateTime? createdAt,
        string createReason)
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
        CreatedAt = createdAt;
        CreateReason = createReason;
    }
}

internal sealed class CreateBusinessCommandHandler : ICommandHandler<CreateBusinessCommand, CreateBusinessResponse>
{
    private readonly ILogger<CreateBusinessCommandHandler> _logger;
    private readonly IBusinessRepository _businessRepository;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CreateBusinessCommandHandler(
        ILogger<CreateBusinessCommandHandler> logger,
        IBusinessRepository businessRepository,
        IMapper mapper, IHttpContextAccessor httpContextAccessor)
    {
        _logger = logger;
        _businessRepository = businessRepository;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Result<CreateBusinessResponse>> Handle(CreateBusinessCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim is null)
            {
                _logger.LogWarning("User ID not found in claims");
                return Result.Failure<CreateBusinessResponse>(new Error("Auth.Unauthorized", "User not authenticated"));
            }

            var userId = userIdClaim.Value;
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("User context is missing for business creation");
                return Result.Failure<CreateBusinessResponse>(new Error("Authorization.Failed",
                    "User context is required"));
            }

            var existingBusiness = await _businessRepository.GetByOwnerIdAsync(userId);
            if (existingBusiness != null)
            {
                _logger.LogWarning("User {UserId} already has a business", userId);
                return Result.Failure<CreateBusinessResponse>(new Error("Business.AlreadyExists",
                    "User already has a business"));
            }

            var business = new Domain.Entities.Business
            {
                Id = Guid.NewGuid(),
                OwnerId = userId,
                Name = request.Name,
                Description = request.Description,
                Address = request.Address,
                Phone = request.Phone,
                Email = request.Email,
                Website = request.Website,
                CreateReason = request.CreateReason,
                IsActive = false,
                IsDisable = false,
                CreatedBy = userId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                ActivatedAt = null
            };

            await _businessRepository.AddAsync(business, cancellationToken);

            var response = _mapper.Map<CreateBusinessResponse>(business);

            _logger.LogInformation("Successfully created business {BusinessId} for user {UserId} - pending approval",
                business.Id, userId);
            return Result.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error creating business");
            return Result.Failure<CreateBusinessResponse>(new Error("InternalError", "An unexpected error occurred"));
        }
    }
}

public class CreateBusinessCommandValidator : AbstractValidator<CreateBusinessCommand>
{
    public CreateBusinessCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Business name is required")
            .MaximumLength(200)
            .WithMessage("Business name cannot exceed 200 characters");

        RuleFor(x => x.CreateReason)
            .NotEmpty()
            .WithMessage("Create reason is required")
            .MaximumLength(1000)
            .WithMessage("Create reason cannot exceed 1000 characters");

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
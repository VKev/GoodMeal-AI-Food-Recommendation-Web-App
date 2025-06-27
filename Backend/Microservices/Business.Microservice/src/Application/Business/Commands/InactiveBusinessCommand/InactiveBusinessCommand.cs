using System.Security.Claims;
using FluentValidation;
using Microsoft.Extensions.Logging;
using SharedLibrary.Common.Messaging;
using SharedLibrary.Common.ResponseModel;
using Domain.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using MassTransit;
using SharedLibrary.Common.Event;

namespace Application.Business.Commands.InactiveBusinessCommand;

public sealed record InactiveBusinessCommand(Guid BusinessId) : ICommand<InactiveBusinessResponse>;

public sealed record InactiveBusinessByAdminCommand(Guid BusinessId, string AdminUserId) : ICommand<InactiveBusinessResponse>;

public sealed class InactiveBusinessResponse
{
    public Guid Id { get; set; }
    public string? OwnerId { get; set; }
    public string Name { get; set; }
    public bool IsActive { get; set; }
    public DateTime? DeactivatedAt { get; set; }
}

internal sealed class InactiveBusinessCommandHandler : ICommandHandler<InactiveBusinessCommand, InactiveBusinessResponse>
{
    private readonly ILogger<InactiveBusinessCommandHandler> _logger;
    private readonly IBusinessRepository _businessRepository;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IPublishEndpoint _publishEndpoint;

    public InactiveBusinessCommandHandler(
        ILogger<InactiveBusinessCommandHandler> logger,
        IBusinessRepository businessRepository,
        IMapper mapper, 
        IHttpContextAccessor httpContextAccessor,
        IPublishEndpoint publishEndpoint)
    {
        _logger = logger;
        _businessRepository = businessRepository;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<Result<InactiveBusinessResponse>> Handle(InactiveBusinessCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                _logger.LogWarning("User ID not found in claims");
                return Result.Failure<InactiveBusinessResponse>(new Error("Auth.InvalidToken", "User not authenticated"));
            }
            
            var userId = userIdClaim.Value;

            var business = await _businessRepository.GetByIdAsync(request.BusinessId);
            if (business == null)
            {
                _logger.LogWarning("Business with ID {BusinessId} not found", request.BusinessId);
                return Result.Failure<InactiveBusinessResponse>(new Error("Business.NotFound", "Business not found"));
            }

            // Kiểm tra quyền owner
            if (business.OwnerId != userId)
            {
                _logger.LogWarning("User {UserId} is not the owner of business {BusinessId}", userId, request.BusinessId);
                return Result.Failure<InactiveBusinessResponse>(new Error("Business.Unauthorized", "You are not authorized to deactivate this business"));
            }

            if (business.IsActive == false)
            {
                _logger.LogWarning("Business {BusinessId} is already inactive", request.BusinessId);
                return Result.Failure<InactiveBusinessResponse>(new Error("Business.AlreadyInactive",
                    "Business is already inactive"));
            }

            var deactivatedAt = DateTime.UtcNow;
            business.IsActive = false;
            business.UpdatedAt = deactivatedAt;

            _businessRepository.Update(business);

            // Publish event khi business được deactivate
            if (!string.IsNullOrEmpty(business.OwnerId))
            {
                var businessDeactivatedEvent = new BusinessDeactivatedEvent
                {
                    BusinessId = business.Id,
                    OwnerId = business.OwnerId,
                    BusinessName = business.Name,
                    DeactivatedAt = deactivatedAt,
                    DeactivatedBy = userId
                };

                await _publishEndpoint.Publish(businessDeactivatedEvent, cancellationToken);
                _logger.LogInformation("Published BusinessDeactivatedEvent for business {BusinessId} and owner {OwnerId}", 
                    business.Id, business.OwnerId);
            }

            var response = _mapper.Map<InactiveBusinessResponse>(business);

            _logger.LogInformation("Successfully deactivated business {BusinessId} by user {UserId}", request.BusinessId,
                userId);
            return Result.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error deactivating business {BusinessId}", request.BusinessId);
            return Result.Failure<InactiveBusinessResponse>(new Error("InternalError", "An unexpected error occurred"));
        }
    }
}

internal sealed class InactiveBusinessByAdminCommandHandler : ICommandHandler<InactiveBusinessByAdminCommand, InactiveBusinessResponse>
{
    private readonly ILogger<InactiveBusinessByAdminCommandHandler> _logger;
    private readonly IBusinessRepository _businessRepository;
    private readonly IMapper _mapper;
    private readonly IPublishEndpoint _publishEndpoint;

    public InactiveBusinessByAdminCommandHandler(
        ILogger<InactiveBusinessByAdminCommandHandler> logger,
        IBusinessRepository businessRepository,
        IMapper mapper,
        IPublishEndpoint publishEndpoint)
    {
        _logger = logger;
        _businessRepository = businessRepository;
        _mapper = mapper;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<Result<InactiveBusinessResponse>> Handle(InactiveBusinessByAdminCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var business = await _businessRepository.GetByIdAsync(request.BusinessId);
            if (business == null)
            {
                _logger.LogWarning("Business with ID {BusinessId} not found", request.BusinessId);
                return Result.Failure<InactiveBusinessResponse>(new Error("Business.NotFound", "Business not found"));
            }

            if (business.IsActive == false)
            {
                _logger.LogWarning("Business {BusinessId} is already inactive", request.BusinessId);
                return Result.Failure<InactiveBusinessResponse>(new Error("Business.AlreadyInactive",
                    "Business is already inactive"));
            }

            var deactivatedAt = DateTime.UtcNow;
            business.IsActive = false;
            business.UpdatedAt = deactivatedAt;

            _businessRepository.Update(business);

            if (!string.IsNullOrEmpty(business.OwnerId))
            {
                var businessDeactivatedEvent = new BusinessDeactivatedEvent
                {
                    BusinessId = business.Id,
                    OwnerId = business.OwnerId,
                    BusinessName = business.Name,
                    DeactivatedAt = deactivatedAt,
                    DeactivatedBy = request.AdminUserId
                };

                await _publishEndpoint.Publish(businessDeactivatedEvent, cancellationToken);
                _logger.LogInformation("Published BusinessDeactivatedEvent for business {BusinessId} and owner {OwnerId}", 
                    business.Id, business.OwnerId);
            }

            var response = _mapper.Map<InactiveBusinessResponse>(business);

            _logger.LogInformation("Successfully deactivated business {BusinessId} by admin {AdminUserId}", 
                request.BusinessId, request.AdminUserId);
            return Result.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error deactivating business {BusinessId}", request.BusinessId);
            return Result.Failure<InactiveBusinessResponse>(new Error("InternalError", "An unexpected error occurred"));
        }
    }
}

public class InactiveBusinessCommandValidator : AbstractValidator<InactiveBusinessCommand>
{
    public InactiveBusinessCommandValidator()
    {
        RuleFor(x => x.BusinessId)
            .NotEmpty()
            .WithMessage("Business ID is required");
    }
}

public class InactiveBusinessByAdminCommandValidator : AbstractValidator<InactiveBusinessByAdminCommand>
{
    public InactiveBusinessByAdminCommandValidator()
    {
        RuleFor(x => x.BusinessId)
            .NotEmpty()
            .WithMessage("Business ID is required");
        
        RuleFor(x => x.AdminUserId)
            .NotEmpty()
            .WithMessage("Admin User ID is required");
    }
} 
using System.Security.Claims;
using FluentValidation;
using Microsoft.Extensions.Logging;
using SharedLibrary.Common.Messaging;
using SharedLibrary.Common.ResponseModel;
using Domain.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using SharedLibrary.Common;
using MassTransit;
using SharedLibrary.Common.Event;

namespace Application.Business.Commands.ActiveBusinessCommand;

public sealed record ActiveBusinessCommand(Guid BusinessId) : ICommand<ActiveBusinessResponse>;

public sealed record ActiveBusinessByAdminCommand(Guid BusinessId, string AdminUserId)
    : ICommand<ActiveBusinessResponse>;

public sealed class ActiveBusinessResponse
{
    public Guid Id { get; set; }
    public string? OwnerId { get; set; }
    public string Name { get; set; }
    public bool IsActive { get; set; }
    public DateTime? ActivatedAt { get; set; }
}

internal sealed class ActiveBusinessCommandHandler : ICommandHandler<ActiveBusinessCommand, ActiveBusinessResponse>
{
    private readonly ILogger<ActiveBusinessCommandHandler> _logger;
    private readonly IBusinessRepository _businessRepository;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IPublishEndpoint _publishEndpoint;

    public ActiveBusinessCommandHandler(
        ILogger<ActiveBusinessCommandHandler> logger,
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

    public async Task<Result<ActiveBusinessResponse>> Handle(ActiveBusinessCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                _logger.LogWarning("User ID not found in claims");
                return Result.Failure<ActiveBusinessResponse>(new Error("Auth.InvalidToken", "User not authenticated"));
            }

            var userId = userIdClaim.Value;

            var business = await _businessRepository.GetByIdAsync(request.BusinessId);
            if (business == null)
            {
                _logger.LogWarning("Business with ID {BusinessId} not found", request.BusinessId);
                return Result.Failure<ActiveBusinessResponse>(new Error("Business.NotFound", "Business not found"));
            }

            // Kiểm tra quyền owner
            if (business.OwnerId != userId)
            {
                _logger.LogWarning("User {UserId} is not the owner of business {BusinessId}", userId,
                    request.BusinessId);
                return Result.Failure<ActiveBusinessResponse>(new Error("Business.Unauthorized",
                    "You are not authorized to activate this business"));
            }

            // Kiểm tra business không bị disable
            if (business.IsDisable == true)
            {
                _logger.LogWarning("Cannot activate disabled business {BusinessId}", request.BusinessId);
                return Result.Failure<ActiveBusinessResponse>(new Error("Business.Disabled",
                    "Cannot activate a disabled business"));
            }

            if (business.IsActive == true)
            {
                _logger.LogWarning("Business {BusinessId} is already active", request.BusinessId);
                return Result.Failure<ActiveBusinessResponse>(new Error("Business.AlreadyActive",
                    "Business is already active"));
            }

            var activatedAt = DateTime.UtcNow;
            business.IsActive = true;
            business.ActivatedAt = activatedAt;
            business.UpdatedAt = activatedAt;

            _businessRepository.Update(business);

            if (!string.IsNullOrEmpty(business.OwnerId))
            {
                var businessActivatedEvent = new BusinessActivatedEvent
                {
                    BusinessId = business.Id,
                    OwnerId = business.OwnerId,
                    BusinessName = business.Name,
                    ActivatedAt = activatedAt,
                    ActivatedBy = userId
                };

                await _publishEndpoint.Publish(businessActivatedEvent, cancellationToken);
                _logger.LogInformation("Published BusinessActivatedEvent for business {BusinessId} and owner {OwnerId}",
                    business.Id, business.OwnerId);
            }

            var response = _mapper.Map<ActiveBusinessResponse>(business);

            _logger.LogInformation("Successfully activated business {BusinessId} by user {UserId}", request.BusinessId,
                userId);
            return Result.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error activating business {BusinessId}", request.BusinessId);
            return Result.Failure<ActiveBusinessResponse>(new Error("InternalError", "An unexpected error occurred"));
        }
    }
}

internal sealed class
    ActiveBusinessByAdminCommandHandler : ICommandHandler<ActiveBusinessByAdminCommand, ActiveBusinessResponse>
{
    private readonly ILogger<ActiveBusinessByAdminCommandHandler> _logger;
    private readonly IBusinessRepository _businessRepository;
    private readonly IMapper _mapper;
    private readonly IPublishEndpoint _publishEndpoint;

    public ActiveBusinessByAdminCommandHandler(
        ILogger<ActiveBusinessByAdminCommandHandler> logger,
        IBusinessRepository businessRepository,
        IMapper mapper,
        IPublishEndpoint publishEndpoint)
    {
        _logger = logger;
        _businessRepository = businessRepository;
        _mapper = mapper;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<Result<ActiveBusinessResponse>> Handle(ActiveBusinessByAdminCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var business = await _businessRepository.GetByIdAsync(request.BusinessId);
            if (business == null)
            {
                _logger.LogWarning("Business with ID {BusinessId} not found", request.BusinessId);
                return Result.Failure<ActiveBusinessResponse>(new Error("Business.NotFound", "Business not found"));
            }

            if (business.IsActive == true)
            {
                _logger.LogWarning("Business {BusinessId} is already active", request.BusinessId);
                return Result.Failure<ActiveBusinessResponse>(new Error("Business.AlreadyActive",
                    "Business is already active"));
            }

            var activatedAt = DateTime.UtcNow;
            business.IsActive = true;
            business.ActivatedAt = activatedAt;
            business.UpdatedAt = activatedAt;

            _businessRepository.Update(business);

            if (!string.IsNullOrEmpty(business.OwnerId))
            {
                var businessActivatedEvent = new BusinessActivatedEvent
                {
                    BusinessId = business.Id,
                    OwnerId = business.OwnerId,
                    BusinessName = business.Name,
                    ActivatedAt = activatedAt,
                    ActivatedBy = request.AdminUserId
                };

                await _publishEndpoint.Publish(businessActivatedEvent, cancellationToken);
                _logger.LogInformation("Published BusinessActivatedEvent for business {BusinessId} and owner {OwnerId}",
                    business.Id, business.OwnerId);
            }

            var response = _mapper.Map<ActiveBusinessResponse>(business);

            _logger.LogInformation("Successfully activated business {BusinessId} by admin {AdminUserId}",
                request.BusinessId, request.AdminUserId);
            return Result.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error activating business {BusinessId}", request.BusinessId);
            return Result.Failure<ActiveBusinessResponse>(new Error("InternalError", "An unexpected error occurred"));
        }
    }
}

public class ActiveBusinessCommandValidator : AbstractValidator<ActiveBusinessCommand>
{
    public ActiveBusinessCommandValidator()
    {
        RuleFor(x => x.BusinessId)
            .NotEmpty()
            .WithMessage("Business ID is required");
    }
}

public class ActiveBusinessByAdminCommandValidator : AbstractValidator<ActiveBusinessByAdminCommand>
{
    public ActiveBusinessByAdminCommandValidator()
    {
        RuleFor(x => x.BusinessId)
            .NotEmpty()
            .WithMessage("Business ID is required");

        RuleFor(x => x.AdminUserId)
            .NotEmpty()
            .WithMessage("Admin User ID is required");
    }
}
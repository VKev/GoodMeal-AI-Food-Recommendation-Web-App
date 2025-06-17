using System.Security.Claims;
using FluentValidation;
using Microsoft.Extensions.Logging;
using SharedLibrary.Common.Messaging;
using SharedLibrary.Common.ResponseModel;
using Domain.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using SharedLibrary.Common;

namespace Application.Business.Commands.DisableBusinessCommand;

public sealed record DisableBusinessCommand(Guid BusinessId) : ICommand<DisableBusinessResponse>;

public sealed record DisableBusinessResponse(
    Guid Id,
    string? OwnerId,
    string Name,
    bool IsDisable,
    DateTime? DisableAt,
    string? DisableBy
);

internal sealed class DisableBusinessCommandHandler : ICommandHandler<DisableBusinessCommand, DisableBusinessResponse>
{
    private readonly ILogger<DisableBusinessCommandHandler> _logger;
    private readonly IBusinessRepository _businessRepository;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public DisableBusinessCommandHandler(
        ILogger<DisableBusinessCommandHandler> logger,
        IBusinessRepository businessRepository,
        IMapper mapper, IHttpContextAccessor httpContextAccessor)
    {
        _logger = logger;
        _businessRepository = businessRepository;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Result<DisableBusinessResponse>> Handle(DisableBusinessCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim is null)
            {
                _logger.LogWarning("User ID not found in claims");
                return Result.Failure<DisableBusinessResponse>(new Error("Auth.Unauthorized",
                    "User not authenticated"));
            }

            var userId = userIdClaim.Value;
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("User context is missing for business disable");
                return Result.Failure<DisableBusinessResponse>(new Error("Authorization.Failed",
                    "User context is required"));
            }

            var business = await _businessRepository.GetByIdAsync(request.BusinessId);
            if (business == null)
            {
                _logger.LogWarning("Business with ID {BusinessId} not found", request.BusinessId);
                return Result.Failure<DisableBusinessResponse>(new Error("Business.NotFound", "Business not found"));
            }

            // Kiểm tra quyền sở hữu
            if (business.OwnerId != userId)
            {
                _logger.LogWarning("User {UserId} is not authorized to disable business {BusinessId}", userId,
                    request.BusinessId);
                return Result.Failure<DisableBusinessResponse>(new Error("Authorization.Failed",
                    "You are not authorized to disable this business"));
            }

            if (business.IsDisable == true)
            {
                _logger.LogWarning("Business {BusinessId} is already disabled", request.BusinessId);
                return Result.Failure<DisableBusinessResponse>(new Error("Business.AlreadyDisabled",
                    "Business is already disabled"));
            }

            business.IsDisable = true;
            business.DisableAt = DateTime.Now;
            business.DisableBy = userId;
            business.UpdatedAt = DateTime.Now;

            _businessRepository.Update(business);

            var response = _mapper.Map<DisableBusinessResponse>(business);

            _logger.LogInformation("Successfully disabled business {BusinessId} by user {UserId}", request.BusinessId,
                userId);
            return Result.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error disabling business {BusinessId}", request.BusinessId);
            return Result.Failure<DisableBusinessResponse>(new Error("InternalError", "An unexpected error occurred"));
        }
    }
}

public class DisableBusinessCommandValidator : AbstractValidator<DisableBusinessCommand>
{
    public DisableBusinessCommandValidator()
    {
        RuleFor(x => x.BusinessId)
            .NotEmpty()
            .WithMessage("Business ID is required");
    }
}
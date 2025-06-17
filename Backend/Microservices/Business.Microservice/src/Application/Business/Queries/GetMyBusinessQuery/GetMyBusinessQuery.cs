using FluentValidation;
using Microsoft.Extensions.Logging;
using SharedLibrary.Common.Messaging;
using SharedLibrary.Common.ResponseModel;
using Domain.Repositories;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using AutoMapper;

namespace Application.Business.Queries.GetMyBusinessQuery;

public sealed record GetMyBusinessQuery() : IQuery<GetMyBusinessResponse>;

public sealed record GetMyBusinessResponse(
    Guid Id,
    string? OwnerId,
    string Name,
    string? Description,
    string? Address,
    string? Phone,
    string? Email,
    string? Website,
    bool IsActive,
    DateTime? CreatedAt,
    DateTime? UpdatedAt
);

internal sealed class GetMyBusinessQueryHandler : IQueryHandler<GetMyBusinessQuery, GetMyBusinessResponse>
{
    private readonly ILogger<GetMyBusinessQueryHandler> _logger;
    private readonly IBusinessRepository _businessRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IMapper _mapper;

    public GetMyBusinessQueryHandler(ILogger<GetMyBusinessQueryHandler> logger, IBusinessRepository businessRepository,
        IHttpContextAccessor httpContextAccessor, IMapper mapper)
    {
        _logger = logger;
        _businessRepository = businessRepository;
        _httpContextAccessor = httpContextAccessor;
        _mapper = mapper;
    }

    public async Task<Result<GetMyBusinessResponse>> Handle(GetMyBusinessQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim is null)
            {
                _logger.LogWarning("User ID not found in claims");
                return Result.Failure<GetMyBusinessResponse>(new Error("Auth.Unauthorized", "User not authenticated"));
            }

            var userId = userIdClaim.Value;
            var business = await _businessRepository.GetByOwnerIdAsync(userId);

            if (business is null)
            {
                _logger.LogWarning("Business not found for user {UserId}", userId);
                return Result.Failure<GetMyBusinessResponse>(new Error("Business.NotFound",
                    "Business not found for current user"));
            }

            var response = _mapper.Map<GetMyBusinessResponse>(business);

            _logger.LogInformation("Successfully retrieved business for user {UserId}", userId);
            return Result.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error getting user's business");
            return Result.Failure<GetMyBusinessResponse>(new Error("InternalError", "An unexpected error occurred"));
        }
    }
}

public class GetMyBusinessQueryValidator : AbstractValidator<GetMyBusinessQuery>
{
    public GetMyBusinessQueryValidator()
    {
        // No validation needed for this query
    }
}
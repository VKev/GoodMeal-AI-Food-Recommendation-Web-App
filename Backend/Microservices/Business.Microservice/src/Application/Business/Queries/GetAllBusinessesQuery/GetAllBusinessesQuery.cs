using FluentValidation;
using Microsoft.Extensions.Logging;
using SharedLibrary.Common.Messaging;
using SharedLibrary.Common.ResponseModel;
using Domain.Repositories;

namespace Application.Business.Queries.GetAllBusinessesQuery;

public sealed record GetAllBusinessesQuery() : IQuery<GetAllBusinessesResponse>;

public sealed record BusinessInfo(
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

public sealed record GetAllBusinessesResponse(
    IEnumerable<BusinessInfo> Businesses,
    int TotalCount
);

internal sealed class GetAllBusinessesQueryHandler : IQueryHandler<GetAllBusinessesQuery, GetAllBusinessesResponse>
{
    private readonly ILogger<GetAllBusinessesQueryHandler> _logger;
    private readonly IBusinessRepository _businessRepository;

    public GetAllBusinessesQueryHandler(ILogger<GetAllBusinessesQueryHandler> logger, IBusinessRepository businessRepository)
    {
        _logger = logger;
        _businessRepository = businessRepository;
    }

    public async Task<Result<GetAllBusinessesResponse>> Handle(GetAllBusinessesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var businesses = await _businessRepository.GetAllAsync();
            
            var businessInfos = businesses.Select(b => new BusinessInfo(
                b.Id,
                b.OwnerId,
                b.Name,
                b.Description,
                b.Address,
                b.Phone,
                b.Email,
                b.Website,
                b.IsActive,
                b.CreatedAt,
                b.UpdatedAt
            ));

            var response = new GetAllBusinessesResponse(businessInfos, businessInfos.Count());

            _logger.LogInformation("Successfully retrieved {Count} businesses", businessInfos.Count());
            return Result.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error getting all businesses");
            return Result.Failure<GetAllBusinessesResponse>(new Error("InternalError", "An unexpected error occurred"));
        }
    }
}

public class GetAllBusinessesQueryValidator : AbstractValidator<GetAllBusinessesQuery>
{
    public GetAllBusinessesQueryValidator()
    {
        // No validation needed for this query
    }
} 
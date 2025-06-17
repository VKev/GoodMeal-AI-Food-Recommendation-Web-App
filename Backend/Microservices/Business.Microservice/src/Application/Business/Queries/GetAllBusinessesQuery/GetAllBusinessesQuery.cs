using FluentValidation;
using Microsoft.Extensions.Logging;
using SharedLibrary.Common.Messaging;
using SharedLibrary.Common.ResponseModel;
using Domain.Repositories;
using AutoMapper;

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
    private readonly IMapper _mapper;

    public GetAllBusinessesQueryHandler(ILogger<GetAllBusinessesQueryHandler> logger, IBusinessRepository businessRepository, IMapper mapper)
    {
        _logger = logger;
        _businessRepository = businessRepository;
        _mapper = mapper;
    }

    public async Task<Result<GetAllBusinessesResponse>> Handle(GetAllBusinessesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var businesses = await _businessRepository.GetAllAsync();
            
            var businessInfos = _mapper.Map<IEnumerable<BusinessInfo>>(businesses);

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
    }
} 
using FluentValidation;
using Microsoft.Extensions.Logging;
using SharedLibrary.Common.Messaging;
using SharedLibrary.Common.ResponseModel;
using Domain.Repositories;
using AutoMapper;

namespace Application.Business.Queries.GetBusinessByIdQuery;

public sealed record GetBusinessByIdQuery(Guid BusinessId) : IQuery<GetBusinessByIdResponse>;

public sealed record GetBusinessByIdResponse(
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

internal sealed class GetBusinessByIdQueryHandler : IQueryHandler<GetBusinessByIdQuery, GetBusinessByIdResponse>
{
    private readonly ILogger<GetBusinessByIdQueryHandler> _logger;
    private readonly IBusinessRepository _businessRepository;
    private readonly IMapper _mapper;

    public GetBusinessByIdQueryHandler(ILogger<GetBusinessByIdQueryHandler> logger, IBusinessRepository businessRepository, IMapper mapper)
    {
        _logger = logger;
        _businessRepository = businessRepository;
        _mapper = mapper;
    }

    public async Task<Result<GetBusinessByIdResponse>> Handle(GetBusinessByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var business = await _businessRepository.GetByIdAsync(request.BusinessId);
            
            if (business is null)
            {
                _logger.LogWarning("Business with ID {BusinessId} not found", request.BusinessId);
                return Result.Failure<GetBusinessByIdResponse>(new Error("Business.NotFound", "Business not found"));
            }

            var response = _mapper.Map<GetBusinessByIdResponse>(business);

            _logger.LogInformation("Successfully retrieved business {BusinessId}", request.BusinessId);
            return Result.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error getting business {BusinessId}", request.BusinessId);
            return Result.Failure<GetBusinessByIdResponse>(new Error("InternalError", "An unexpected error occurred"));
        }
    }
}

public class GetBusinessByIdQueryValidator : AbstractValidator<GetBusinessByIdQuery>
{
    public GetBusinessByIdQueryValidator()
    {
        RuleFor(x => x.BusinessId)
            .NotEmpty()
            .WithMessage("Business ID is required");
    }
} 
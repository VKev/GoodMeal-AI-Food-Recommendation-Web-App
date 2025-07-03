using MassTransit;
using SharedLibrary.Contracts.Business;
using Application.Business.Queries.GetAllBusinessesQuery;
using MediatR;
using AutoMapper;
using Microsoft.Extensions.Logging;
using GetAllBusinessesResponse = SharedLibrary.Contracts.Business.GetAllBusinessesResponse;

namespace Application.Consumers;

public class GetAllBusinessesConsumer : IConsumer<GetAllBusinessesRequest>
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;
    private readonly ILogger<GetAllBusinessesConsumer> _logger;

    public GetAllBusinessesConsumer(IMediator mediator, IMapper mapper, ILogger<GetAllBusinessesConsumer> logger)
    {
        _mediator = mediator;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<GetAllBusinessesRequest> context)
    {
        try
        {
            _logger.LogInformation("Received GetAllBusinessesRequest with RequestId: {RequestId}",
                context.Message.RequestId);

            var result = await _mediator.Send(new GetAllBusinessesQuery());

            if (result.IsFailure)
            {
                await context.RespondAsync(new GetAllBusinessesResponse
                {
                    RequestId = context.Message.RequestId,
                    IsSuccess = false,
                    ErrorMessage = result.Error.Description,
                    Businesses = new List<BusinessDto>()
                });
                return;
            }

            var businessDtos = result.Value.Businesses.Select(b => _mapper.Map<BusinessDto>(b)).ToList();

            await context.RespondAsync(new GetAllBusinessesResponse
            {
                RequestId = context.Message.RequestId,
                IsSuccess = true,
                ErrorMessage = null,
                Businesses = businessDtos
            });

            _logger.LogInformation("Successfully processed GetAllBusinessesRequest with RequestId: {RequestId}",
                context.Message.RequestId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing GetAllBusinessesRequest with RequestId: {RequestId}",
                context.Message.RequestId);

            await context.RespondAsync(new GetAllBusinessesResponse
            {
                RequestId = context.Message.RequestId,
                IsSuccess = false,
                ErrorMessage = "An unexpected error occurred",
                Businesses = new List<BusinessDto>()
            });
        }
    }
}
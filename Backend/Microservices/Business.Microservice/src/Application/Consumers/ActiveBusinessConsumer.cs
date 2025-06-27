using MassTransit;
using SharedLibrary.Contracts.Business;
using Application.Business.Commands.ActiveBusinessCommand;
using Application.Business.Queries.GetBusinessByIdQuery;
using MediatR;
using AutoMapper;
using Microsoft.Extensions.Logging;
using SharedLibrary.Common.Messaging.Commands;
using SharedLibrary.Common;
using ActiveBusinessResponse = SharedLibrary.Contracts.Business.ActiveBusinessResponse;

namespace Application.Consumers;

public class ActiveBusinessConsumer : IConsumer<ActiveBusinessRequest>
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;
    private readonly ILogger<ActiveBusinessConsumer> _logger;

    public ActiveBusinessConsumer(IMediator mediator, IMapper mapper, ILogger<ActiveBusinessConsumer> logger)
    {
        _mediator = mediator;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<ActiveBusinessRequest> context)
    {
        try
        {
            _logger.LogInformation(
                "Received ActiveBusinessRequest with RequestId: {RequestId} for BusinessId: {BusinessId}",
                context.Message.RequestId, context.Message.BusinessId);

            var adminUserId = !string.IsNullOrEmpty(context.Message.UserId)
                ? context.Message.UserId
                : "SYSTEM";

            var activeResult = await _mediator.Send(new ActiveBusinessByAdminCommand(
                context.Message.BusinessId,
                adminUserId));
            var saveResult = await _mediator.Send(new SaveChangesCommand());
            var getBusinessResult = await _mediator.Send(new GetBusinessByIdQuery(context.Message.BusinessId));

            var aggregatedResult = ResultAggregator.AggregateWithNumbers(
                (activeResult, true),
                (saveResult, false),
                (getBusinessResult, true)
            );

            if (aggregatedResult.IsFailure)
            {
                await context.RespondAsync(new ActiveBusinessResponse
                {
                    RequestId = context.Message.RequestId,
                    IsSuccess = false,
                    ErrorMessage = aggregatedResult.Error.Description,
                    Business = null
                });
                return;
            }

            var businessDto = _mapper.Map<BusinessDto>(getBusinessResult.Value);

            await context.RespondAsync(new ActiveBusinessResponse
            {
                RequestId = context.Message.RequestId,
                IsSuccess = true,
                ErrorMessage = null,
                Business = businessDto
            });

            _logger.LogInformation(
                "Successfully processed ActiveBusinessRequest with RequestId: {RequestId} for BusinessId: {BusinessId}",
                context.Message.RequestId, context.Message.BusinessId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error processing ActiveBusinessRequest with RequestId: {RequestId} for BusinessId: {BusinessId}",
                context.Message.RequestId, context.Message.BusinessId);

            await context.RespondAsync(new ActiveBusinessResponse
            {
                RequestId = context.Message.RequestId,
                IsSuccess = false,
                ErrorMessage = "An unexpected error occurred",
                Business = null
            });
        }
    }
}
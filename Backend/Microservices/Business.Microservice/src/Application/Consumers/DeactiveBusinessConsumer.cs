using MassTransit;
using SharedLibrary.Contracts.Business;
using Application.Business.Commands.DisableBusinessCommand;
using Application.Business.Queries.GetBusinessByIdQuery;
using MediatR;
using AutoMapper;
using Microsoft.Extensions.Logging;
using SharedLibrary.Common.Messaging.Commands;
using SharedLibrary.Common;

namespace Application.Consumers;

public class DeactiveBusinessConsumer : IConsumer<DeactiveBusinessRequest>
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;
    private readonly ILogger<DeactiveBusinessConsumer> _logger;

    public DeactiveBusinessConsumer(IMediator mediator, IMapper mapper, ILogger<DeactiveBusinessConsumer> logger)
    {
        _mediator = mediator;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<DeactiveBusinessRequest> context)
    {
        try
        {
            _logger.LogInformation(
                "Received DisableBusinessRequest with RequestId: {RequestId} for BusinessId: {BusinessId}",
                context.Message.RequestId, context.Message.BusinessId);

            var adminUserId = !string.IsNullOrEmpty(context.Message.UserId)
                ? context.Message.UserId
                : "SYSTEM";

            var disableResult = await _mediator.Send(new DisableBusinessByAdminCommand(
                context.Message.BusinessId,
                adminUserId));
            var saveResult = await _mediator.Send(new SaveChangesCommand());
            var getBusinessResult = await _mediator.Send(new GetBusinessByIdQuery(context.Message.BusinessId));

            var aggregatedResult = ResultAggregator.AggregateWithNumbers(
                (disableResult, true),
                (saveResult, false),
                (getBusinessResult, true)
            );

            if (aggregatedResult.IsFailure)
            {
                await context.RespondAsync(new DeactiveBusinessResponse
                {
                    RequestId = context.Message.RequestId,
                    IsSuccess = false,
                    ErrorMessage = aggregatedResult.Error.Description,
                    Business = null
                });
                return;
            }

            var businessDto = _mapper.Map<BusinessDto>(aggregatedResult.Value);

            await context.RespondAsync(new DeactiveBusinessResponse
            {
                RequestId = context.Message.RequestId,
                IsSuccess = true,
                ErrorMessage = null,
                Business = businessDto
            });

            _logger.LogInformation(
                "Successfully processed DisableBusinessRequest with RequestId: {RequestId} for BusinessId: {BusinessId}",
                context.Message.RequestId, context.Message.BusinessId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error processing DisableBusinessRequest with RequestId: {RequestId} for BusinessId: {BusinessId}",
                context.Message.RequestId, context.Message.BusinessId);

            await context.RespondAsync(new DeactiveBusinessResponse
            {
                RequestId = context.Message.RequestId,
                IsSuccess = false,
                ErrorMessage = "An unexpected error occurred",
                Business = null
            });
        }
    }
}
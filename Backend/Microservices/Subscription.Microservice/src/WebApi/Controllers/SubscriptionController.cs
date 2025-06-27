using MediatR;
using Microsoft.AspNetCore.Mvc;
using Application.Subscriptions.Queries.GetAllSubscriptionsQuery;
using Application.Subscriptions.Queries.GetSubscriptionByIdQuery;
using Application.Subscriptions.Commands.CreateSubscriptionCommand;
using Application.Subscriptions.Commands.UpdateSubscriptionCommand;
using Application.Subscriptions.Commands.DeleteSubscriptionCommand;
using Application.UserSubscriptions.Commands.SubscribeUserCommand;
using Application.UserSubscriptions.Queries.GetMySubscriptionQuery;
using SharedLibrary.Common;
using SharedLibrary.Common.Messaging.Commands;
using SharedLibrary.Utils.AuthenticationExtention;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    public class SubscriptionController : ApiController
    {
        public SubscriptionController(IMediator mediator) : base(mediator)
        {
        }

        [HttpGet]
        [ApiGatewayUser]
        public async Task<IActionResult> GetAllSubscriptions(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetAllSubscriptionsQuery(), cancellationToken);
            if (result.IsFailure)
            {
                return HandleFailure(result);
            }

            return Ok(result);
        }

        [HttpGet("{subscriptionId}")]
        [ApiGatewayUser]
        public async Task<IActionResult> GetSubscriptionById(Guid subscriptionId, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetSubscriptionByIdQuery(subscriptionId), cancellationToken);
            if (result.IsFailure)
            {
                return HandleFailure(result);
            }

            return Ok(result);
        }

        [HttpPost]
        [ApiGatewayUser]
        public async Task<IActionResult> CreateSubscription([FromBody] CreateSubscriptionCommand command,
            CancellationToken cancellationToken)
        {
            var createResult = await _mediator.Send(command, cancellationToken);
            var saveResult = await _mediator.Send(new SaveChangesCommand(), cancellationToken);
            var getSubscriptionResult =
                await _mediator.Send(new GetSubscriptionByIdQuery(createResult.Value?.Id ?? Guid.Empty),
                    cancellationToken);

            var aggregatedResult = ResultAggregator.AggregateWithNumbers(
                (createResult, false),
                (saveResult, false),
                (getSubscriptionResult, true)
            );

            if (aggregatedResult.IsFailure)
            {
                return HandleFailure(aggregatedResult);
            }

            return Ok(aggregatedResult.Value);
        }

        [HttpPut("{subscriptionId}")]
        [ApiGatewayUser]
        public async Task<IActionResult> UpdateSubscription(Guid subscriptionId,
            [FromBody] UpdateSubscriptionRequest request,
            CancellationToken cancellationToken)
        {
            var command = new UpdateSubscriptionCommand(
                subscriptionId,
                request.Name,
                request.Description,
                request.Price,
                request.DurationInMonths,
                request.Currency
            );

            var updateResult = await _mediator.Send(command, cancellationToken);
            var saveResult = await _mediator.Send(new SaveChangesCommand(), cancellationToken);
            var getSubscriptionResult =
                await _mediator.Send(new GetSubscriptionByIdQuery(subscriptionId), cancellationToken);

            var aggregatedResult = ResultAggregator.AggregateWithNumbers(
                (updateResult, false),
                (saveResult, false),
                (getSubscriptionResult, true)
            );

            if (aggregatedResult.IsFailure)
            {
                return HandleFailure(aggregatedResult);
            }

            return Ok(aggregatedResult.Value);
        }

        [HttpDelete("{subscriptionId}")]
        [ApiGatewayUser]
        public async Task<IActionResult> DeleteSubscription(Guid subscriptionId, CancellationToken cancellationToken)
        {
            var deleteResult = await _mediator.Send(new DeleteSubscriptionCommand(subscriptionId), cancellationToken);
            var saveResult = await _mediator.Send(new SaveChangesCommand(), cancellationToken);
            var getSubscriptionResult =
                await _mediator.Send(new GetSubscriptionByIdQuery(subscriptionId), cancellationToken);

            var aggregatedResult = ResultAggregator.AggregateWithNumbers(
                (deleteResult, false),
                (saveResult, false),
                (getSubscriptionResult, true)
            );

            if (aggregatedResult.IsFailure)
            {
                return HandleFailure(aggregatedResult);
            }

            return Ok(aggregatedResult.Value);
        }
        
        [HttpGet("my-subscription")]
        [ApiGatewayUser]
        public async Task<IActionResult> GetMySubscription(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetMySubscriptionQuery(), cancellationToken);
            if (result.IsFailure)
            {
                return HandleFailure(result);
            }

            return Ok(result);
        }

        [HttpGet("health")]
        public async Task<IActionResult> Health()
        {
            return Ok();
        }
    }

    public record UpdateSubscriptionRequest(
        string Name,
        string? Description,
        decimal Price,
        int DurationInMonths,
        string Currency
    );
}
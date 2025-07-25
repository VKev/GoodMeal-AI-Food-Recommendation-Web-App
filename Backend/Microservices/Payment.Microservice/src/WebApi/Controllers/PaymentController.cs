using MediatR;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.Common;
using Application.Payments.Commands;
using Application.Payments.Queries;
using MassTransit;
using VNPAY.NET.Utilities;

namespace WebApi.Controllers;

[Route("api/[controller]")]
public class PaymentController : ApiController
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<PaymentController> _logger;

    public PaymentController(IMediator mediator, IPublishEndpoint publishEndpoint, ILogger<PaymentController> logger) :
        base(mediator)
    {
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    [HttpPost("CreatePaymentUrl")]
    public async Task<IActionResult> CreatePaymentUrl(
        [FromBody] CreatePaymentUrlCommand request,
        CancellationToken cancellationToken)
    {
        var ipAddress = NetworkHelper.GetIpAddress(HttpContext);

        var command = new CreatePaymentUrlCommand(
            request.Amount,
            request.OrderDescription,
            request.OrderId,
            ipAddress
        );

        var result = await _mediator.Send(command, cancellationToken);

        var aggregatedResult = ResultAggregator.Aggregate(result);
        if (aggregatedResult.IsFailure)
        {
            return HandleFailure(aggregatedResult);
        }

        return Ok(aggregatedResult);
    }

    [HttpGet("CheckStatus/{orderId}/{transactionDate}")]
    public async Task<IActionResult> CheckPaymentStatus(
        string orderId,
        string transactionDate,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Received payment status check request for OrderId: {OrderId}", orderId);

        var query = new CheckPaymentStatusQuery(orderId, transactionDate);

        var result = await _mediator.Send(query, cancellationToken);

        var aggregatedResult = ResultAggregator.AggregateWithNumbers((result, true));
        if (aggregatedResult.IsFailure)
        {
            _logger.LogWarning("Payment status check failed for OrderId: {OrderId}. Error: {Error}",
                orderId, aggregatedResult.Error);
            return HandleFailure(aggregatedResult);
        }

        _logger.LogInformation("Payment status check completed for OrderId: {OrderId}. Status: {Status}",
            orderId, result.Value.Message);

        return Ok(aggregatedResult);
    }


    [HttpGet("IpnAction")]
    public async Task<IActionResult> IpnAction(CancellationToken cancellationToken)
    {
        if (Request.QueryString.HasValue)
        {
            _logger.LogInformation("Received IPN request with query string: {QueryString}", Request.QueryString.Value);
            var command = new ProcessIpnCommand(Request.Query);

            var result = await _mediator.Send(command, cancellationToken);

            if (result.IsSuccess && result.Value)
            {
                return Ok();
            }

            var error = result.IsSuccess ? "Thanh toán thất bại" : result.Error.Description;

            return BadRequest(error);
        }

        return NotFound("Không tìm thấy thông tin thanh toán.");
    }

    [HttpGet("health")]
    public async Task<IActionResult> Health(CancellationToken cancellationToken)
    {
        return Ok();
    }
}
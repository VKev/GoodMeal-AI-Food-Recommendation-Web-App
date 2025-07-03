using Domain.Repositories;
using SharedLibrary.Common.ResponseModel;
using SharedLibrary.Common.Messaging;
using Microsoft.Extensions.Logging;

namespace Application.Payments.Commands;

public sealed record CreatePaymentUrlCommand(
    decimal Amount,
    string OrderDescription,
    string OrderId,
    string IpAddress
) : ICommand<CreatePaymentUrlResponse>;

public sealed record CreatePaymentUrlResponse(
    string PaymentUrl,
    string OrderId
);

internal sealed class
    CreatePaymentUrlCommandHandler : ICommandHandler<CreatePaymentUrlCommand, CreatePaymentUrlResponse>
{
    private readonly IVnpayRepository _vnpayRepository;
    private readonly ILogger<CreatePaymentUrlCommandHandler> _logger;

    public CreatePaymentUrlCommandHandler(
        IVnpayRepository vnpayRepository,
        ILogger<CreatePaymentUrlCommandHandler> logger)
    {
        _vnpayRepository = vnpayRepository;
        _logger = logger;
    }

    public async Task<Result<CreatePaymentUrlResponse>> Handle(CreatePaymentUrlCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            // Use IP address from command parameter (passed from saga or HTTP context)
            var ipAddress = !string.IsNullOrEmpty(request.IpAddress) ? request.IpAddress : "127.0.0.1";
            _logger.LogDebug("Using IP address: {IpAddress} for order {OrderId}", ipAddress, request.OrderId);

            var paymentUrl = _vnpayRepository.CreatePaymentUrl(
                request.Amount,
                request.OrderDescription,
                request.OrderId,
                ipAddress);

            var response = new CreatePaymentUrlResponse(paymentUrl, request.OrderId);
            return Result.Success(response);
        }
        catch (Exception ex)
        {
            return Result.Failure<CreatePaymentUrlResponse>(new Error("PaymentUrl.Creation", ex.Message));
        }
    }
}
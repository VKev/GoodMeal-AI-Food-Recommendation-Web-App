using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SharedLibrary.Common.ResponseModel;
using VNPAY.NET;

namespace Application.Payments.Commands;

public class ProcessIpnCommandHandler : IRequestHandler<ProcessIpnCommand, Result>
{
    private readonly ILogger<ProcessIpnCommandHandler> _logger;

    private readonly IVnpay _vnpay;

    public ProcessIpnCommandHandler(ILogger<ProcessIpnCommandHandler> logger, IVnpay vnpay)
    {
        _logger = logger;
        _vnpay = vnpay;
    }

    public async Task<Result> Handle(ProcessIpnCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Processing IPN with parameters: {Parameters}", request.QueryParameters);

            var paymentResult = _vnpay.GetPaymentResult(request.QueryParameters);

            if (paymentResult.IsSuccess)
            {
                _logger.LogInformation("Payment successful for OrderId: {OrderId}",
                    request.QueryParameters["vnp_TxnRef"].ToString());

                return Result.Success(true);
            }

            _logger.LogWarning("Payment failed for OrderId: {OrderId}. Message: {Message}",
                request.QueryParameters["vnp_TxnRef"].ToString(),
                paymentResult.Description);

            return Result.Success(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing IPN");
            return Result.Failure(Error.FromException(ex));
        }
    }
}
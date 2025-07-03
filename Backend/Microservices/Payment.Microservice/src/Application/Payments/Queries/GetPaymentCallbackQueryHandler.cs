using Application.Payments.Commands;
using Domain.Repositories;
using SharedLibrary.Common.ResponseModel;
using SharedLibrary.Common.Messaging;
using Microsoft.AspNetCore.Http;

namespace Application.Payments.Queries;

public sealed record GetPaymentCallbackQuery(
    IQueryCollection QueryParameters
) : IQuery<PaymentCallbackResponse>;

public sealed record PaymentCallbackResponse(
    bool IsSuccess,
    string Description,
    string TransactionStatus,
    string OrderId
);

internal sealed class GetPaymentCallbackQueryHandler : IQueryHandler<GetPaymentCallbackQuery, PaymentCallbackResponse>
{
    private readonly IVnpayRepository _vnpayRepository;

    public GetPaymentCallbackQueryHandler(IVnpayRepository vnpayRepository)
    {
        _vnpayRepository = vnpayRepository;
    }

    public async Task<Result<PaymentCallbackResponse>> Handle(GetPaymentCallbackQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            if (!request.QueryParameters.Any())
            {
                return Result.Failure<PaymentCallbackResponse>(new Error("Callback.Invalid",
                    "Không tìm thấy thông tin thanh toán."));
            }

            var paymentResult = _vnpayRepository.GetPaymentResult(request.QueryParameters);
            var orderId = request.QueryParameters["vnp_TxnRef"].FirstOrDefault() ?? "";

            var response = new PaymentCallbackResponse(
                paymentResult.IsSuccess,
                paymentResult.PaymentResponse.Description,
                paymentResult.TransactionStatus.Description,
                orderId
            );

            return Result.Success(response);
        }
        catch (Exception ex)
        {
            return Result.Failure<PaymentCallbackResponse>(new Error("Callback.Processing", ex.Message));
        }
    }
}
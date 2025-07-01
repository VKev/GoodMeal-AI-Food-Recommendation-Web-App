using Domain.Repositories;
using SharedLibrary.Common.ResponseModel;
using SharedLibrary.Common.Messaging;
using Microsoft.AspNetCore.Http;
using VNPAY.NET.Utilities;

namespace Application.Payments.Commands;

public sealed record CreatePaymentUrlCommand(
    decimal Amount,
    string OrderDescription,
    string OrderId
) : ICommand<CreatePaymentUrlResponse>;

public sealed record CreatePaymentUrlResponse(
    string PaymentUrl,
    string OrderId
);

internal sealed class
    CreatePaymentUrlCommandHandler : ICommandHandler<CreatePaymentUrlCommand, CreatePaymentUrlResponse>
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IVnpayRepository _vnpayRepository;

    public CreatePaymentUrlCommandHandler(IHttpContextAccessor httpContextAccessor, IVnpayRepository vnpayRepository)
    {
        _httpContextAccessor = httpContextAccessor;
        _vnpayRepository = vnpayRepository;
    }

    public async Task<Result<CreatePaymentUrlResponse>> Handle(CreatePaymentUrlCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var ipAddress = NetworkHelper.GetIpAddress(_httpContextAccessor.HttpContext!);

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
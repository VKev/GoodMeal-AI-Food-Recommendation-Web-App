using Domain.Repositories;
using SharedLibrary.Common.ResponseModel;
using SharedLibrary.Common.Messaging;
using Microsoft.AspNetCore.Http;

namespace Application.Payments.Commands;

public sealed record ProcessIpnCommand(
    IQueryCollection QueryParameters
) : ICommand<ProcessIpnResponse>;

public sealed record ProcessIpnResponse(
    bool IsSuccess,
    string Message
);

internal sealed class ProcessIpnCommandHandler : ICommandHandler<ProcessIpnCommand, ProcessIpnResponse>
{
    private readonly IVnpayRepository _vnpayRepository;

    public ProcessIpnCommandHandler(IVnpayRepository vnpayRepository)
    {
        _vnpayRepository = vnpayRepository;
    }

    public async Task<Result<ProcessIpnResponse>> Handle(ProcessIpnCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (!request.QueryParameters.Any())
            {
                return Result.Failure<ProcessIpnResponse>(new Error("Ipn.Invalid",
                    "Không tìm thấy thông tin thanh toán."));
            }

            var paymentResult = _vnpayRepository.GetPaymentResult(request.QueryParameters);
            if (paymentResult.IsSuccess)
            {
                var response = new ProcessIpnResponse(true, "Thanh toán thành công");
                return Result.Success(response);
            }

            return Result.Failure<ProcessIpnResponse>(new Error("Payment.Failed", "Thanh toán thất bại"));
        }
        catch (Exception ex)
        {
            return Result.Failure<ProcessIpnResponse>(new Error("Ipn.Processing", ex.Message));
        }
    }
}
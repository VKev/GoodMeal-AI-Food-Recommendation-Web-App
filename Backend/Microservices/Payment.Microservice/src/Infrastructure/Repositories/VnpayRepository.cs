using Domain.Repositories;
using VNPAY.NET;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using VNPAY.NET.Enums;
using VNPAY.NET.Models;

namespace Infrastructure.Services;

public class VnpayRepository : IVnpayRepository
{
    private readonly IVnpay _vnpay;
    private readonly IConfiguration _configuration;

    public VnpayRepository(IVnpay vnpay, IConfiguration configuration)
    {
        _vnpay = vnpay;
        _configuration = configuration;

        var tmnCode = Environment.GetEnvironmentVariable("VNPAYTMNCODE");
        var hashSecret = Environment.GetEnvironmentVariable("VNPAYHASHSECRET");
        var baseUrl = Environment.GetEnvironmentVariable("VNPAYBASEURL");
        var returnUrl = Environment.GetEnvironmentVariable("VNPAYRETURNURL");

        _vnpay.Initialize(tmnCode!, hashSecret!, baseUrl!, returnUrl!);
    }

    public string CreatePaymentUrl(decimal amount, string orderDescription, string orderId, string ipAddress)
    {
        var request = new PaymentRequest
        {
            PaymentId = DateTime.Now.Ticks,
            Money = (double)amount,
            Description = orderDescription,
            IpAddress = ipAddress,
            BankCode = BankCode.ANY,
            CreatedDate = DateTime.Now,
            Currency = Currency.VND,
            Language = DisplayLanguage.Vietnamese
        };

        return _vnpay.GetPaymentUrl(request);
    }

    public PaymentResult GetPaymentResult(IQueryCollection queryCollection)
    {
        return _vnpay.GetPaymentResult(queryCollection);
    }
}
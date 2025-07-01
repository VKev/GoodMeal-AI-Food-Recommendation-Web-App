using Microsoft.AspNetCore.Http;
using VNPAY.NET.Models;

namespace Domain.Repositories;

public interface IVnpayRepository
{
    string CreatePaymentUrl(decimal amount, string orderDescription, string orderId, string ipAddress);
    PaymentResult GetPaymentResult(IQueryCollection queryCollection);
}
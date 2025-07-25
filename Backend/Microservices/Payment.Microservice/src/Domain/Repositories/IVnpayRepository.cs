using Microsoft.AspNetCore.Http;
using VNPAY.NET.Models;

namespace Domain.Repositories;

public interface IVnpayRepository
{
    (string, string) CreatePaymentUrl(decimal amount, string orderDescription, string orderId, string ipAddress);
    Task<PaymentResult> GetPaymentResult(string orderId, string transactionDate);
}
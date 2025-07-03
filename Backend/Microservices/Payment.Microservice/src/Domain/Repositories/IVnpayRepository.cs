using Microsoft.AspNetCore.Http;
using VNPAY.NET.Models;

namespace Domain.Repositories;

public interface IVnpayRepository
{
    string CreatePaymentUrl(decimal amount, string orderDescription, string orderId, string ipAddress);
    PaymentResult GetPaymentResult(IQueryCollection queryCollection);
    Task<PaymentStatusResult> QueryPaymentStatusAsync(string orderId, string transactionDate);
}

public record PaymentStatusResult(
    bool IsSuccess,
    string Status,
    string Message,
    decimal? Amount,
    string? TransactionId,
    DateTime? TransactionDate
);
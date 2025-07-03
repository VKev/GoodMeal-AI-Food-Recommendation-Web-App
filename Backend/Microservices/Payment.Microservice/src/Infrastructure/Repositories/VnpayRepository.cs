using Domain.Repositories;
using VNPAY.NET;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using VNPAY.NET.Enums;
using VNPAY.NET.Models;
using System.Text;
using System.Security.Cryptography;
using System.Text.Json;

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
            PaymentId = DateTime.UtcNow.Ticks,
            Money = (double)amount,
            Description = orderDescription,
            IpAddress = ipAddress,
            BankCode = BankCode.ANY,
            CreatedDate = DateTime.UtcNow,
            Currency = Currency.VND,
            Language = DisplayLanguage.Vietnamese
        };

        return _vnpay.GetPaymentUrl(request);
    }

    public PaymentResult GetPaymentResult(IQueryCollection queryCollection)
    {
        return _vnpay.GetPaymentResult(queryCollection);
    }

    public async Task<PaymentStatusResult> QueryPaymentStatusAsync(string orderId, string transactionDate)
    {
        try
        {
            var tmnCode = Environment.GetEnvironmentVariable("VNPAYTMNCODE");
            var hashSecret = Environment.GetEnvironmentVariable("VNPAYHASHSECRET");
            var queryUrl = Environment.GetEnvironmentVariable("VNPAYQUERYURL") ?? "https://sandbox.vnpayment.vn/querydr/PaymentVerify.aspx";

            if (string.IsNullOrEmpty(tmnCode) || string.IsNullOrEmpty(hashSecret))
            {
                return new PaymentStatusResult(
                    IsSuccess: false,
                    Status: "CONFIG_ERROR",
                    Message: "VnPay configuration is missing",
                    Amount: null,
                    TransactionId: null,
                    TransactionDate: null
                );
            }

            var requestData = new Dictionary<string, string>
            {
                ["vnp_RequestId"] = Guid.NewGuid().ToString(),
                ["vnp_Version"] = "2.1.0",
                ["vnp_Command"] = "querydr",
                ["vnp_TmnCode"] = tmnCode,
                ["vnp_TxnRef"] = orderId,
                ["vnp_OrderInfo"] = $"Query payment status for order {orderId}",
                ["vnp_TransactionNo"] = "",
                ["vnp_TransactionDate"] = transactionDate,
                ["vnp_CreateDate"] = DateTime.UtcNow.ToString("yyyyMMddHHmmss"),
                ["vnp_IpAddr"] = "127.0.0.1"
            };

            // Create signature
            var sortedParams = requestData.OrderBy(x => x.Key).ToList();
            var queryString = string.Join("&", sortedParams.Select(x => $"{x.Key}={x.Value}"));
            var signature = CreateSignature(queryString, hashSecret);
            requestData["vnp_SecureHash"] = signature;

            // Make HTTP request to VnPay
            using var httpClient = new HttpClient();
            var formData = new FormUrlEncodedContent(requestData);
            
            var response = await httpClient.PostAsync(queryUrl, formData);
            var responseContent = await response.Content.ReadAsStringAsync();

            // Parse response (VnPay returns form-encoded response)
            var responseData = ParseVnPayResponse(responseContent);

            if (responseData.ContainsKey("vnp_ResponseCode") && responseData["vnp_ResponseCode"] == "00")
            {
                var isSuccess = responseData.ContainsKey("vnp_TransactionStatus") && responseData["vnp_TransactionStatus"] == "00";
                decimal? amount = null;
                if (responseData.ContainsKey("vnp_Amount") && decimal.TryParse(responseData["vnp_Amount"], out var amountValue))
                {
                    amount = amountValue / 100; // VnPay returns amount in smallest unit
                }

                DateTime? transactionDateTime = null;
                if (responseData.ContainsKey("vnp_PayDate") && DateTime.TryParseExact(responseData["vnp_PayDate"], "yyyyMMddHHmmss", null, System.Globalization.DateTimeStyles.None, out var payDate))
                {
                    transactionDateTime = payDate;
                }

                return new PaymentStatusResult(
                    IsSuccess: isSuccess,
                    Status: isSuccess ? "SUCCESS" : "FAILED",
                    Message: responseData.GetValueOrDefault("vnp_Message", "Payment status queried successfully"),
                    Amount: amount,
                    TransactionId: responseData.GetValueOrDefault("vnp_TransactionNo"),
                    TransactionDate: transactionDateTime
                );
            }
            else
            {
                return new PaymentStatusResult(
                    IsSuccess: false,
                    Status: "QUERY_FAILED",
                    Message: responseData.GetValueOrDefault("vnp_Message", "Failed to query payment status"),
                    Amount: null,
                    TransactionId: null,
                    TransactionDate: null
                );
            }
        }
        catch (Exception ex)
        {
            return new PaymentStatusResult(
                IsSuccess: false,
                Status: "ERROR",
                Message: $"Error querying payment status: {ex.Message}",
                Amount: null,
                TransactionId: null,
                TransactionDate: null
            );
        }
    }

    private string CreateSignature(string data, string secretKey)
    {
        using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(secretKey));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
        return Convert.ToHexString(hash).ToLower();
    }

    private Dictionary<string, string> ParseVnPayResponse(string response)
    {
        var result = new Dictionary<string, string>();
        
        try
        {
            var jsonDoc = JsonDocument.Parse(response);
            foreach (var property in jsonDoc.RootElement.EnumerateObject())
            {
                result[property.Name] = property.Value.GetString() ?? "";
            }
        }
        catch
        {
            var pairs = response.Split('&');
            foreach (var pair in pairs)
            {
                var keyValue = pair.Split('=', 2);
                if (keyValue.Length == 2)
                {
                    result[keyValue[0]] = Uri.UnescapeDataString(keyValue[1]);
                }
            }
        }

        return result;
    }
}
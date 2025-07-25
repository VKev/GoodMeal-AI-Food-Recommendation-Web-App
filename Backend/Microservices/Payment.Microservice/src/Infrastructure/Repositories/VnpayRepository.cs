using Domain.Repositories;
using System.Text;
using System.Security.Cryptography;
using System.Text.Json;
using System.Net;
using System.Globalization;
using Infrastructure.Configs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VNPAY.NET;
using VNPAY.NET.Models;

namespace Infrastructure.Services;

public class VnpayRepository : IVnpayRepository
{
    private readonly HttpClient _httpClient;
    private readonly SortedList<string, string> _requestData = new SortedList<string, string>(new VnPayCompare());
    private readonly ILogger<VnpayRepository> _logger;
    private readonly IOptions<AppConfig> _appConfig;
    private readonly IVnpay _vnpay;

    public VnpayRepository(HttpClient httpClient, ILogger<VnpayRepository> logger, IOptions<AppConfig> appConfig,
        IVnpay vnpay)
    {
        _httpClient = httpClient;
        _logger = logger;
        _appConfig = appConfig;

        _vnpay = vnpay;
        _vnpay.Initialize(_appConfig.Value.TmnCode, _appConfig.Value.HashSecret, _appConfig.Value.VnpayApiUrl,
            _appConfig.Value.VnpayCallBackUrl);
    }


    public (string, string) CreatePaymentUrl(decimal amount, string orderDescription, string orderId, string ipAddress)
    {
        string date = DateTime.Now.ToString("yyyyMMddHHmmss");
        AddRequestData("vnp_Version", "2.1.0");
        AddRequestData("vnp_Command", "pay");
        AddRequestData("vnp_TmnCode", _appConfig.Value.TmnCode);
        AddRequestData("vnp_Amount", MumberToString(amount));
        AddRequestData("vnp_BankCode", "");
        AddRequestData("vnp_CreateDate", date);
        AddRequestData("vnp_CurrCode", "VND");
        AddRequestData("vnp_IpAddr", ipAddress);
        AddRequestData("vnp_Locale", "vn");
        AddRequestData("vnp_OrderInfo", "Payment for " + orderId);
        AddRequestData("vnp_OrderType", "other");
        AddRequestData("vnp_ReturnUrl", $"{_appConfig.Value.VnpayCallBackUrl}&orderId={orderId}");
        AddRequestData("vnp_TxnRef", orderId);

        string paymentUrl = CreateRequestUrl(_appConfig.Value.VnpayApiUrl, _appConfig.Value.HashSecret);
        return (paymentUrl, date);
    }

    public async Task<PaymentResult> GetPaymentResult(string orderId, string transactionDate)
    {
        _requestData.Clear();
        string requestId = DateTime.Now.Ticks.ToString();
        string createDate = DateTime.Now.ToString("yyyyMMddHHmmss");
        string clientIp = GetClientIPAddress();

        AddRequestData("vnp_RequestId", requestId);
        AddRequestData("vnp_Version", "2.1.0");
        AddRequestData("vnp_Command", "querydr");
        AddRequestData("vnp_TmnCode", _appConfig.Value.TmnCode);
        AddRequestData("vnp_TxnRef", orderId);
        AddRequestData("vnp_OrderInfo", $"Query transaction {orderId}");
        AddRequestData("vnp_TransactionDate", transactionDate);
        AddRequestData("vnp_CreateDate", createDate);
        AddRequestData("vnp_IpAddr", clientIp);

        // Create hash according to VNPAY documentation
        string data =
            $"{requestId}|2.1.0|querydr|{_appConfig.Value.TmnCode}|{orderId}|{transactionDate}|{createDate}|{clientIp}|Query transaction {orderId}";
        string secureHash = HmacSHA512(_appConfig.Value.HashSecret, data);

        // Create the request body as a dictionary
        var requestBody = new Dictionary<string, string>
        {
            { "vnp_RequestId", requestId },
            { "vnp_Version", "2.1.0" },
            { "vnp_Command", "querydr" },
            { "vnp_TmnCode", _appConfig.Value.TmnCode },
            { "vnp_TxnRef", orderId },
            { "vnp_OrderInfo", $"Query transaction {orderId}" },
            { "vnp_TransactionDate", transactionDate },
            { "vnp_CreateDate", createDate },
            { "vnp_IpAddr", clientIp },
            { "vnp_SecureHash", secureHash }
        };

        var content = new StringContent(
            JsonSerializer.Serialize(requestBody),
            Encoding.UTF8,
            "application/json"
        );

        _logger.LogInformation("Sending VNPAY QueryDr request for order {OrderId}: {Request}", orderId,
            JsonSerializer.Serialize(requestBody));

        var response =
            await _httpClient.PostAsync("https://sandbox.vnpayment.vn/merchant_webapi/api/transaction", content);
        var result = await response.Content.ReadAsStringAsync();

        try
        {
            var jsonDoc = JsonDocument.Parse(result);
            var root = jsonDoc.RootElement;

            string resultCode = "99"; // Default error code
            string message = "Unknown response";
            long vnpayTransactionId = 0;

            if (root.TryGetProperty("vnp_ResponseCode", out var responseCodeElement))
            {
                resultCode = responseCodeElement.GetString() ?? "99";
            }

            if (root.TryGetProperty("vnp_Message", out var messageElement))
            {
                message = messageElement.GetString() ?? "Unknown response";
            }

            if (root.TryGetProperty("vnp_TransactionNo", out var transactionElement))
            {
                long.TryParse(transactionElement.GetString(), out vnpayTransactionId);
            }

            _logger.LogInformation("VNPAY API response for order {OrderId}: {Response}", orderId, result);

            return new PaymentResult
            {
                PaymentId = 0,
                IsSuccess = resultCode == "00",
                Description = message,
                Timestamp = DateTime.UtcNow,
                VnpayTransactionId = vnpayTransactionId,
                PaymentMethod = "VNPAY"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error parsing VNPAY API response: {Response}", result);
            return new PaymentResult
            {
                PaymentId = 0,
                IsSuccess = false,
                Description = $"Error parsing response: {ex.Message}",
                Timestamp = DateTime.UtcNow,
                VnpayTransactionId = 0,
                PaymentMethod = "VNPAY"
            };
        }
    }

    private void AddRequestData(string key, string value)
    {
        if (!String.IsNullOrEmpty(value))
        {
            _requestData[key] = value;
        }
    }

    private string CreateRequestUrl(string baseUrl, string vnp_HashSecret)
    {
        StringBuilder data = new StringBuilder();
        foreach (KeyValuePair<string, string> kv in _requestData)
        {
            if (!String.IsNullOrEmpty(kv.Value))
            {
                data.Append(WebUtility.UrlEncode(kv.Key) + "=" + WebUtility.UrlEncode(kv.Value) + "&");
            }
        }

        string queryString = data.ToString();

        baseUrl += "?" + queryString;
        String signData = queryString;
        if (signData.Length > 0)
        {
            signData = signData.Remove(data.Length - 1, 1);
        }

        string vnp_SecureHash = HmacSHA512(vnp_HashSecret, signData);
        baseUrl += "vnp_SecureHash=" + vnp_SecureHash;

        return baseUrl;
    }

    private string GetClientIPAddress()
    {
        try
        {
            string hostName = Dns.GetHostName();
            IPAddress[] addresses = Dns.GetHostAddresses(hostName);
            IPAddress? ipv4Address =
                addresses.FirstOrDefault(a => a.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
            return ipv4Address?.ToString() ?? "127.0.0.1";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting client IP address");
            return "127.0.0.1";
        }
    }

    private static string HmacSHA512(string key, string inputData)
    {
        var hash = new StringBuilder();
        byte[] keyBytes = Encoding.UTF8.GetBytes(key);
        byte[] inputBytes = Encoding.UTF8.GetBytes(inputData);
        using (var hmac = new HMACSHA512(keyBytes))
        {
            byte[] hashValue = hmac.ComputeHash(inputBytes);
            foreach (var theByte in hashValue)
            {
                hash.Append(theByte.ToString("x2"));
            }
        }

        return hash.ToString();
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
            // If it's not valid JSON, try parsing as form-encoded
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

    static string MumberToString(decimal value)
    {
        int roundedValue = (int)Math.Round(value) * 100;
        return roundedValue.ToString();
    }
}

public class VnPayCompare : IComparer<string>
{
    public int Compare(string x, string y)
    {
        if (x == y) return 0;
        if (x == null) return -1;
        if (y == null) return 1;
        var vnpCompare = CompareInfo.GetCompareInfo("en-US");
        return vnpCompare.Compare(x, y, CompareOptions.Ordinal);
    }
}
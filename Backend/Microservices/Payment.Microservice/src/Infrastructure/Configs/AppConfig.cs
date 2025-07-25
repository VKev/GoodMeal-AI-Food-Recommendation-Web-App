using System;

namespace Infrastructure.Configs
{
    public class AppConfig
    {
        public string BaseUrl { get; set; } = "http://localhost:3000/my-subscription";
        public string TmnCode { get; set; } = Environment.GetEnvironmentVariable("VNPAYTMNCODE") ?? "default";
        public string HashSecret { get; set; } = Environment.GetEnvironmentVariable("VNPAYHASHSECRET") ?? "default";
        public string VnpayApiUrl { get; set; } = Environment.GetEnvironmentVariable("VNPAYBASEURL") ?? "default";
        public string VnpayCallBackUrl { get; set; } = Environment.GetEnvironmentVariable("VNPAYRETURNURL") ?? "default";
    }
} 
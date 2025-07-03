using System.Net;
using System.Net.Sockets;
using Microsoft.AspNetCore.Http;

namespace SharedLibrary.Utils;

public class NetworkHelper
{
    public static string GetIpAddress(HttpContext context)
    {
        IPAddress remoteIpAddress = context.Connection.RemoteIpAddress;
        if (remoteIpAddress == null)
            throw new NullReferenceException("Không tìm thấy địa chỉ IP");
        return remoteIpAddress.AddressFamily != AddressFamily.InterNetworkV6 ? remoteIpAddress.ToString() : ((IEnumerable<IPAddress>) Dns.GetHostEntry(remoteIpAddress).AddressList).FirstOrDefault<IPAddress>((Func<IPAddress, bool>) (x => x.AddressFamily == AddressFamily.InterNetwork)).ToString();
    }
}

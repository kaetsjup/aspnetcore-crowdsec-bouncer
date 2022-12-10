using System.Net;

namespace AspNetCore.CrowdSec;

public interface ICrowdSecApiClient
{
    Task<IpAddressQueryResult> QueryIpAddressAsync(IPAddress ipAddress);
}
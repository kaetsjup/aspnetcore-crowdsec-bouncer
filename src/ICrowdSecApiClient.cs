using System.Net;

namespace AspNetCore.CrowdSec.Bouncer;

public interface ICrowdSecApiClient
{
    Task<IpAddressQueryResult> QueryIpAddressAsync(IPAddress ipAddress);
}
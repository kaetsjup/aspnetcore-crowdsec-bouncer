namespace AspNetCore.CrowdSec;

public class IpAddressQueryResult 
    : Enumeration
{
    public static readonly IpAddressQueryResult Unknown = new(1, "ok");
    public static readonly IpAddressQueryResult Banned = new(2, "ban");
    public static readonly IpAddressQueryResult Challenge = new(3, "challenge");


    private IpAddressQueryResult(int id, string name) 
        : base(id, name)
    {
    }
}
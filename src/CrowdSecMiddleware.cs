using System.Net;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AspNetCore.CrowdSec.Bouncer;

public class CrowdSecMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<CrowdSecMiddleware> _logger;
    private readonly ICrowdSecApiClient _crowdSecApiClient;
    private readonly CrowdSecOptions _options;

    public CrowdSecMiddleware(RequestDelegate next,
        ILogger<CrowdSecMiddleware> logger,
        ICrowdSecApiClient crowdSecApiClient,
        IOptions<CrowdSecOptions> options)
    {
        _next = next;
        _logger = logger;
        _crowdSecApiClient = crowdSecApiClient;
        _options = options.Value;
    }
    
    public async Task InvokeAsync(HttpContext context)
    {
        _logger.LogInformation($"{context.Connection.RemoteIpAddress} requesting " +
                               $"resource: {context.Request.GetEncodedUrl()}");
        
        var ipAddress = context.Connection.RemoteIpAddress;
        var queryResult = await _crowdSecApiClient.QueryIpAddressAsync(ipAddress);

        if (queryResult == IpAddressQueryResult.Unknown)
        {
            await _next(context);
        }
        else
        {
            if (queryResult == IpAddressQueryResult.Challenge)
                ChallengeAccess(context);
            
            if (queryResult == IpAddressQueryResult.Banned)
                await DenyAccess(context);

            await Task.CompletedTask;
        }
    }

    private Task DenyAccess(HttpContext context)
    {
        _logger.LogWarning($"Request from {context.Connection.RemoteIpAddress} was " +
                           "blocked by the CrowdSec API. " +
                           $"Resource requested: {context.Request.GetEncodedUrl()}. ");

        context.Response.ContentType = "text/html";
        context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
        return context.Response.WriteAsync("Forbidden - Request blocked by CrowdSec.", Encoding.UTF8);
    }

    private void ChallengeAccess(HttpContext context)
    {
        _logger.LogWarning($"Request from {context.Connection.RemoteIpAddress} was " +
                           "challenged by the CrowdSec API. " +
                           $"Resource requested: {context.Request.GetEncodedUrl()}. ");
        
        context.Response.Redirect(_options.CaptchaChallengeUrl.ToString(),
            permanent: false);
    }
}
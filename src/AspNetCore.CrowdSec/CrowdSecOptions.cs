namespace AspNetCore.CrowdSec;

public class CrowdSecOptions
{
    /// <summary>
    /// The CrowdSec Local API key.
    /// </summary>
    public string ApiKey { get; set; } = string.Empty;
    
    /// <summary>
    /// The URI for the CrowdSec API server. The default is http://localhost
    /// </summary>
    public Uri ApiServerUrl { get; set; } = new("http://localhost");
    
    /// <summary>
    /// CrowdSec API request timeout. The default is 10 minutes.
    /// </summary>
    public TimeSpan ApiServerRequestTimeout { get; set; } = TimeSpan.FromMinutes(10);

    // TODO (kaetsjup): this needs refactoring.
    public Uri CaptchaChallengeUrl { get; set; } = new("/captcha.html");
}
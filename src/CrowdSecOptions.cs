namespace AspNetCore.CrowdSec;

public class CrowdSecOptions
{
    public string ApiKey { get; set; } = string.Empty;
    public Uri ApiServerUrl { get; set; } = new("http://localhost");
    public TimeSpan ApiServerRequestTimeout { get; set; } = TimeSpan.FromMinutes(10);
    public Uri CaptchaChallengeUrl { get; set; } = new("/captcha.html");
}
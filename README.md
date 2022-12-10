# WARNING:
This is work-in-progress and therefore unstable!

# CrowdSec Bouncer for ASP.NET Core
A ASP.NET Core [CrowdSec bouncer](https://docs.crowdsec.net/docs/bouncers/intro) implementation written in C#.

This middlware can be used for blocking or challenging malicious traffic using the [CrowdSec](https://crowdsec.net/) API.

## Usage
Install the middleware using [NuGet](https://www.nuget.org/) (Pending):

```csharp
dotnet add package AspNetCore.CrowdSec
```

then bootstrap the middleware in your ASP.NET Core application using the following options as minimum required:

```csharp
using AspNetCore.CrowdSec;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCrowdSecBouncer(
    new CrowdSecOptions
    {
        ApiKey = "<YOUR_CROWDSEC_API_KEY>",
        ApiServerUrl = new Uri("http(s)://URL_FOR_THE_CROWDSEC_API/"),
        CaptchaChallengeUrl = new Uri("http(s)://URL_FOR_YOUR_CAPTCHA_CHALLENGE_PAGE/"),
    });

var app = builder.Build();
app.UseCrowdSecBouncer();
app.Run();
```

## Configuration
The ASP.NET Core CrowdSec middleware Bouncer supports the following options:

| Name                       | Description                                                                                                        | Default                 |
|----------------------------|--------------------------------------------------------------------------------------------------------------------|-------------------------|
| `ApiKey`                   | CrowdSec bouncer API key                                                                                           | `(empty string)`        |
| `ApiServerUrl`             | URL for the CrowdSec Local API agent                                                                               | `http://localhost`      |
| `ApiServerRequestTimeout`  | The timespan to wait before the HTTP request times out                                                             | `10 Minutes`            |
| `CaptchaChallengeUrl`      | URL for the captcha challenge (Can be relative or absolute).                                                       | `/captcha.html`         |

## Contributing
Please fork the repository and make changes as you'd like. Pull requests are more than welcome.


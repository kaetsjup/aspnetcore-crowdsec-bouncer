using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AspNetCore.CrowdSec;

public class CrowdSecApiClient 
    : ICrowdSecApiClient
{
    private readonly IHttpClientFactory _httpClientFactory;

    public CrowdSecApiClient(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
    }

    public async Task<IpAddressQueryResult> QueryIpAddressAsync(IPAddress ipAddress)
    {
        if (ipAddress == null) throw new ArgumentNullException(nameof(ipAddress));
        var client = _httpClientFactory.CreateClient("crowdsec-api-client");

        var request = new HttpRequestMessage(HttpMethod.Get, $"decisions/?ip={ipAddress}");
        var response = await client.SendAsync(request);
        var content = await response.Content.ReadAsStringAsync();
        
        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException(
                "No successful response was returned by the CrowdSec API. " +
                $"CrowdSec API Response: {content}");
        }

        if (string.IsNullOrEmpty(content))
            return IpAddressQueryResult.Unknown;
        
        var query = JsonSerializer.Deserialize<ApiQueryResult>(content);

        // TODO (kaetsjup): this needs refactoring.
        return query?.Type switch
        {
            "ban" => IpAddressQueryResult.Banned,
            "captcha" => IpAddressQueryResult.Challenge,
            _ => IpAddressQueryResult.Unknown
        };
    }
}

internal class ApiQueryResult
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    [JsonPropertyName("origin")]
    public string Origin { get; set; }
    [JsonPropertyName("type")]
    public string Type { get; set; }
    [JsonPropertyName("value")]
    public string Value { get; set; }
    [JsonPropertyName("duration")]
    public string Duration { get; set; }
}
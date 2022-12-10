using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.CrowdSec;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCrowdSecBouncer(this IServiceCollection serviceCollection, CrowdSecOptions options)
    {
        if (serviceCollection == null) throw new ArgumentNullException(nameof(serviceCollection));
        if (options == null) throw new ArgumentNullException(nameof(options));
        
        EnsureConfigurationIsValid(options);

        serviceCollection.AddHttpClient("crowdsec-api-client",
            client =>
            {
                client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "aspnet-crowdsec-bouncer/1.0");
                client.DefaultRequestHeaders.TryAddWithoutValidation("X-Api-Key", options.ApiKey);
                client.BaseAddress = options.ApiServerUrl;
                client.Timeout = options.ApiServerRequestTimeout;
            });
        
        // TODO (kaetsjup): this needs refactoring.
        serviceCollection.Configure<CrowdSecOptions>(o =>
            {
                o.ApiKey = options.ApiKey;
                o.ApiServerRequestTimeout = options.ApiServerRequestTimeout;
                o.ApiServerUrl = options.ApiServerUrl;
            });
        
        serviceCollection.AddTransient<ICrowdSecApiClient, CrowdSecApiClient>();
        return serviceCollection;
    }
    
    private static void EnsureConfigurationIsValid(CrowdSecOptions options)
    {
        if (string.IsNullOrEmpty(options.ApiKey))
            throw new InvalidOperationException($"Required option {nameof(CrowdSecOptions.ApiKey)} is null or empty.");
        
        if (options.ApiServerUrl == null)
            throw new InvalidOperationException($"Required option '{nameof(CrowdSecOptions.ApiServerUrl)}' is null.");
    }
}
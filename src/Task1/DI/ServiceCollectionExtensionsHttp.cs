using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Task1.Domain;
using Task1.Http;

namespace Task1.DI;

public static class ServiceCollectionExtensionsHttp
{
    public static IServiceCollection AddConfigClientHttp(this IServiceCollection services)
    {
        services.AddHttpClient<IConfigClient, HttpConfigClient>((provider, client) =>
        {
            IOptionsMonitor<ConfigClientOptions> options = provider.GetRequiredService<IOptionsMonitor<ConfigClientOptions>>();
            client.BaseAddress = new Uri(options.CurrentValue.BaseAddress);
        });

        return services;
    }
}
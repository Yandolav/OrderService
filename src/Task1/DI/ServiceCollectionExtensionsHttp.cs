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
            IOptions<ConfigClientOptions> options = provider.GetRequiredService<IOptions<ConfigClientOptions>>();
            string? baseAddress = options.Value.BaseAddress;

            if (string.IsNullOrWhiteSpace(baseAddress)) throw new InvalidOperationException("BaseAddress is not configured in ConfigClientOptions.");

            client.BaseAddress = new Uri(baseAddress);
        });

        return services;
    }
}
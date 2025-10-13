using Microsoft.Extensions.DependencyInjection;
using Task1.Domain;
using Task1.Http;

namespace Task1.DI;

public static class ServiceCollectionExtensionsHttp
{
    public static IServiceCollection AddConfigClientHttp(this IServiceCollection services, Uri baseUri)
    {
        services.AddHttpClient(ConfigClientDefaults.HttpClientName, c => c.BaseAddress = baseUri);
        services.AddTransient<IConfigClient, HttpConfigClient>();
        return services;
    }
}
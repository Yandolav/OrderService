using Microsoft.Extensions.DependencyInjection;
using Task1.Domain;
using Task1.Http;

namespace Task1.DI;

public static class ServiceCollectionExtensionsHttp
{
    public static IServiceCollection AddConfigClientHttp(this IServiceCollection services, Uri baseUri)
    {
        services.AddHttpClient();
        services.AddTransient<IConfigClient>(sp => new HttpConfigClient(sp.GetRequiredService<IHttpClientFactory>(), baseUri));
        return services;
    }
}
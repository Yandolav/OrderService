using Microsoft.Extensions.DependencyInjection;
using Refit;
using Task1.Domain;
using Task1.Refit;

namespace Task1.DI;

public static class ServiceCollectionExtensionsRefit
{
    public static IServiceCollection AddConfigClientRefit(this IServiceCollection services, Uri baseAddress)
    {
        services.AddRefitClient<IConfigurationsApi>().ConfigureHttpClient(c => c.BaseAddress = baseAddress);
        services.AddTransient<IConfigClient, RefitConfigClient>();
        return services;
    }
}
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Refit;
using Task1.Domain;
using Task1.Refit;

namespace Task1.DI;

public static class ServiceCollectionExtensionsRefit
{
    public static IServiceCollection AddConfigClientRefit(this IServiceCollection services)
    {
        services.AddRefitClient<IConfigurationsApi>()
            .ConfigureHttpClient((provider, client) =>
            {
                IOptionsMonitor<ConfigClientOptions> options = provider.GetRequiredService<IOptionsMonitor<ConfigClientOptions>>();
                client.BaseAddress = new Uri(options.CurrentValue.BaseAddress);
            });

        services.AddTransient<IConfigClient, RefitConfigClient>();

        return services;
    }
}
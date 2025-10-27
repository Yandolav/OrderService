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
                IOptions<ConfigClientOptions> options = provider.GetRequiredService<IOptions<ConfigClientOptions>>();
                string? baseAddress = options.Value.BaseAddress;

                if (string.IsNullOrWhiteSpace(baseAddress)) throw new InvalidOperationException("BaseAddress is not configured in ConfigClientOptions.");

                client.BaseAddress = new Uri(baseAddress);
            });

        services.AddTransient<IConfigClient, RefitConfigClient>();

        return services;
    }
}
using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Persistence.Extensions;

public static class ServiceProviderExtensions
{
    public static void RunMigrations(this IServiceProvider serviceProvider)
    {
        using AsyncServiceScope scope = serviceProvider.CreateAsyncScope();
        IMigrationRunner runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
        runner.MigrateUp();
    }
}
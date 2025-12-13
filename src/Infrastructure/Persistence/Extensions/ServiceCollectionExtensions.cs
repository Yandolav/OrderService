using Core.Application.Ports.PrimaryPorts;
using Core.Application.Ports.SecondaryPorts;
using Core.Application.Services;
using Core.Domain.Enums;
using FluentMigrator.Runner;
using Infrastructure.Persistence.Migrations;
using Infrastructure.Persistence.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Npgsql;

namespace Infrastructure.Persistence.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IProductsRepository, ProductsRepository>();
        services.AddScoped<IOrdersRepository, OrdersRepository>();
        services.AddScoped<IOrderItemsRepository, OrderItemsRepository>();
        services.AddScoped<IOrderHistoryRepository, OrderHistoryRepository>();
        return services;
    }

    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IProductsService, ProductsService>();
        services.AddScoped<IOrdersService, OrdersService>();
        services.AddScoped<IOrderHistoryService, OrderHistoryService>();
        services.AddSingleton(TimeProvider.System);
        return services;
    }

    public static IServiceCollection AddPostgres(this IServiceCollection services)
    {
        services.AddSingleton(serviceProvider =>
        {
            IOptions<DatabaseOptions> option = serviceProvider.GetRequiredService<IOptions<DatabaseOptions>>();
            string connectionString = option.Value.ConnectionString ?? throw new InvalidOperationException("BaseAddress is not configured in ConfigClientOptions.");
            var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
            dataSourceBuilder.MapEnum<OrderHistoryItemKind>(pgName: "order_history_item_kind");
            dataSourceBuilder.MapEnum<OrderState>(pgName: "order_state");
            return dataSourceBuilder.Build();
        });
        return services;
    }

    public static IServiceCollection AddMigrations(this IServiceCollection services)
    {
        services
            .AddFluentMigratorCore()
            .ConfigureRunner(runner => runner
                .AddPostgres()
                .WithGlobalConnectionString(serviceProvider => serviceProvider.GetRequiredService<IOptions<DatabaseOptions>>().Value.ConnectionString ?? throw new InvalidOperationException("BaseAddress is not configured in ConfigClientOptions."))
                .WithMigrationsIn(typeof(IMigrationAssemblyMarker).Assembly))
            .AddLogging(loggingBuilder => loggingBuilder.AddFluentMigratorConsole());
        return services;
    }
}
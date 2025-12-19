using Core.Contracts.OrderHistory;
using Core.Contracts.Orders;
using Core.Contracts.Products;
using Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IProductsService, ProductsService>();
        services.AddScoped<IOrdersService, OrdersService>();
        services.AddScoped<IOrderHistoryService, OrderHistoryService>();
        services.AddSingleton(TimeProvider.System);
        return services;
    }
}
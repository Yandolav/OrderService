using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Task3.HttpGateway.GrpcClient;
using Task3.HttpGateway.Mappings;
using Task3.HttpGateway.Middleware;
using Task3.HttpGateway.Swagger;

namespace Task3.HttpGateway.DI;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddHttpGateway(this IServiceCollection services)
    {
        services.AddControllers().AddJsonOptions(o => { o.JsonSerializerOptions.WriteIndented = false; });
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Store HTTP Gateway",
                Version = "v1",
            });
            c.SchemaFilter<PolymorphismSchemaFilter>();
        });

        services.AddSingleton<IGrpcOrdersClientFactory, GrpcOrdersClientFactory>();
        services.AddSingleton<GrpcMapper>();
        services.AddSingleton<GrpcExceptionMiddleware>();

        return services;
    }
}
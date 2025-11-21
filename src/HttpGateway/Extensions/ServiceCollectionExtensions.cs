using HttpGateway.Mappings;
using HttpGateway.Middleware;
using HttpGateway.Options;
using HttpGateway.Swagger;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Presentation.Grpc;

namespace HttpGateway.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddHttpGateway(this IServiceCollection services)
    {
        services.AddControllers();
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

        services.AddGrpcClient<OrderService.OrderServiceClient>((provider, options) =>
        {
            IOptions<GrpcClientOptions> grpcOptions = provider.GetRequiredService<IOptions<GrpcClientOptions>>();
            if (string.IsNullOrWhiteSpace(grpcOptions.Value.Url)) throw new InvalidOperationException("GrpcClient:Url is not configured.");
            options.Address = new Uri(grpcOptions.Value.Url);
        });
        services.AddSingleton<GrpcMapper>();
        services.AddSingleton<GrpcExceptionMiddleware>();

        return services;
    }
}
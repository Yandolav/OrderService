using Microsoft.Extensions.DependencyInjection;
using Presentation.Grpc.Interceptors;

namespace Presentation;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGrpcPresentation(this IServiceCollection services)
    {
        services.AddGrpc(options =>
        {
            options.Interceptors.Add<ErrorHandlingInterceptor>();
        });
        return services;
    }
}
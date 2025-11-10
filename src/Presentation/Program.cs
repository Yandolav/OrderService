using Infrastructure.Persistence;
using Infrastructure.Persistence.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Presentation.BackgroundServices;
using Presentation.Extensions;
using Presentation.Grpc;
using Presentation.Options;
using Task1;
using Task1.DI;
using Task1.Domain;
using Task2;
using Task2.Provider;
using Task2.Service;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
IConfigurationBuilder configuration = builder.Configuration;

var externalProvider = new ExternalConfigurationProvider();
externalProvider.TryApplyItems(new[]
{
    new ConfigurationItem("ConfigClient:BaseAddress", "http://localhost:8080"),
    new ConfigurationItem("ConfigClient:PageSize", "50"),
    new ConfigurationItem("ConfigRefresh:IntervalSeconds", "1"),
    new ConfigurationItem("GrpcServer:Url", "http://0.0.0.0:8090"),
});
configuration.Add(new ExternalConfigurationSource(externalProvider));

builder.Services.Configure<ConfigClientOptions>(builder.Configuration.GetSection("ConfigClient"));
builder.Services.Configure<ConfigRefreshOptions>(builder.Configuration.GetSection("ConfigRefresh"));
builder.Services.AddSingleton(externalProvider);
builder.Services.AddConfigClientRefit();
builder.Services.AddSingleton<IConfigRefreshService, ConfigRefreshService>();

builder.Services.Configure<DatabaseOptions>(builder.Configuration.GetSection("Database"));
builder.Services
    .AddPostgres()
    .AddRepositories()
    .AddApplicationServices()
    .AddMigrations();

builder.Services.AddHostedService<ConfigRefreshBackgroundService>();
builder.Services.Configure<GrpcServerOptions>(builder.Configuration.GetSection("GrpcServer"));
builder.Services.AddGrpcPresentation();
builder.WebHost.ConfigureGrpcHost(builder.Configuration);

WebApplication app = builder.Build();
app.MapGrpcService<OrdersGrpcService>();
app.Run();
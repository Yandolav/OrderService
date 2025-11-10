using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Presentation.Hosting;
using Task1;
using Task1.DI;
using Task1.Domain;
using Task2;
using Task2.Provider;
using Task2.Service;
using Task3.HttpGateway.DI;
using Task3.HttpGateway.Middleware;
using Task3.HttpGateway.Options;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
IConfigurationBuilder configuration = builder.Configuration;

var externalProvider = new ExternalConfigurationProvider();
externalProvider.TryApplyItems(new[]
{
    new ConfigurationItem("ConfigClient:BaseAddress", "http://localhost:8080"),
    new ConfigurationItem("ConfigClient:PageSize", "50"),
    new ConfigurationItem("ConfigRefresh:IntervalSeconds", "1"),
    new ConfigurationItem("HttpServer:Url", "http://0.0.0.0:8081"),
    new ConfigurationItem("GrpcClient:Url", "http://localhost:8090"),
});
configuration.Add(new ExternalConfigurationSource(externalProvider));

builder.Services.Configure<ConfigClientOptions>(builder.Configuration.GetSection("ConfigClient"));
builder.Services.Configure<ConfigRefreshOptions>(builder.Configuration.GetSection("ConfigRefresh"));
builder.Services.AddSingleton(externalProvider);
builder.Services.AddConfigClientRefit();
builder.Services.AddSingleton<IConfigRefreshService, ConfigRefreshService>();
builder.Services.AddHostedService<ConfigRefreshBackgroundService>();

builder.Services.Configure<HttpServerOptions>(builder.Configuration.GetSection("HttpServer"));
builder.Services.Configure<GrpcClientOptions>(builder.Configuration.GetSection("GrpcClient"));
builder.Services.AddHttpGateway();
builder.WebHost.ConfigureHttpHost(builder.Configuration);

WebApplication app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseMiddleware<GrpcExceptionMiddleware>();
app.MapControllers();

app.Run();
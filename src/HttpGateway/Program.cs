using HttpGateway.Extensions;
using HttpGateway.Middleware;
using HttpGateway.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Presentation.BackgroundServices;
using Task1;
using Task1.DI;
using Task2;
using Task2.Provider;
using Task2.Service;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
IConfigurationBuilder configuration = builder.Configuration;

var externalProvider = new ExternalConfigurationProvider();
configuration.AddJsonFile("appsettings.json");
configuration.Add(new ExternalConfigurationSource(externalProvider));

builder.Services.Configure<ConfigClientOptions>(builder.Configuration.GetSection("ConfigClient"));
builder.Services.Configure<ConfigRefreshOptions>(builder.Configuration.GetSection("ConfigRefresh"));
builder.Services.AddSingleton(externalProvider);
builder.Services.AddConfigClientRefit();
builder.Services.AddSingleton<IConfigRefreshService, ConfigRefreshService>();
builder.Services.AddHostedService<ConfigRefreshBackgroundService>();

builder.Services.Configure<HttpServerOptions>(builder.Configuration.GetSection("HttpServer"));
builder.Services.Configure<GrpcClientOptions>(builder.Configuration.GetSection("GrpcClient"));
builder.Services.Configure<ProcessingServiceGrpcOptions>(builder.Configuration.GetSection("ProcessingServiceGrpc"));
builder.Services.AddHttpGateway();

builder.WebHost.ConfigureHttpHost(builder.Configuration);

WebApplication app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseMiddleware<GrpcExceptionMiddleware>();
app.MapControllers();

app.Run();
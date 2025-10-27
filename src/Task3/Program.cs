using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Task1;
using Task1.DI;
using Task2;
using Task2.Provider;
using Task2.Service;
using Task3;
using Task3.Options;

var cts = new CancellationTokenSource();
Console.CancelKeyPress += (s, e) =>
{
    e.Cancel = true;
    cts.Cancel();
};

var manager = new ConfigurationManager();
IConfigurationBuilder builder = manager;

var externalProvider = new ExternalConfigurationProvider();
builder.Add(new ExternalConfigurationSource(externalProvider));

var services = new ServiceCollection();
services.AddLogging(b =>
{
    b.AddConsole();
    b.AddFilter("System.Net.Http.HttpClient", LogLevel.None);
});
services.Configure<DisplayOptions>(manager.GetSection("Display"));
services.Configure<ConfigClientOptions>(manager.GetSection("ConfigClient"));
services.Configure<ConfigRefreshOptions>(manager.GetSection("ConfigRefresh"));
services.AddSingleton(externalProvider);
ConfigClientOptions clientOptsForRegistration = manager.GetSection("ConfigClient").Get<ConfigClientOptions>() ?? new ConfigClientOptions();

if (string.Equals(clientOptsForRegistration.Implementation, "Refit", StringComparison.OrdinalIgnoreCase))
    services.AddConfigClientRefit();
else
    services.AddConfigClientHttp();
services.AddSingleton<IConfigRefreshService, ConfigRefreshService>();
services.AddSingleton<IDisplayRenderer, DisplayRenderer>();

using ServiceProvider sp = services.BuildServiceProvider();

IConfigRefreshService refresh = sp.GetRequiredService<IConfigRefreshService>();
_ = refresh.RunPeriodicRefreshAsync(cts.Token);

IDisplayRenderer renderer = sp.GetRequiredService<IDisplayRenderer>();
await renderer.RunAsync(cts.Token);
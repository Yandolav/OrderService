using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Task1.DI;
using Task2.Provider;
using Task2.Service;
using Task3;

var cts = new CancellationTokenSource();
Console.CancelKeyPress += (s, e) =>
{
    e.Cancel = true;
    cts.Cancel();
};

var manager = new ConfigurationManager();
IConfigurationBuilder builder = manager;

var externalProvider = new ExternalConfigurationProvider();
builder
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .Add(new ExternalConfigurationSource(externalProvider));

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

var baseUri = new Uri(clientOptsForRegistration.BaseAddress);
if (string.Equals(clientOptsForRegistration.Implementation, "Refit", StringComparison.OrdinalIgnoreCase))
    services.AddConfigClientRefit(baseUri);
else
    services.AddConfigClientHttp(baseUri);
services.AddSingleton<IConfigRefreshService, ConfigRefreshService>();
services.AddSingleton<IDisplayRenderer, DisplayRenderer>();

using ServiceProvider sp = services.BuildServiceProvider();
ConfigClientOptions cfgClient = sp.GetRequiredService<IOptions<ConfigClientOptions>>().Value;
ConfigRefreshOptions cfgRefresh = sp.GetRequiredService<IOptions<ConfigRefreshOptions>>().Value;

IConfigRefreshService refresh = sp.GetRequiredService<IConfigRefreshService>();
_ = refresh.RunPeriodicRefreshAsync(TimeSpan.FromSeconds(cfgRefresh.IntervalSeconds), cfgClient.PageSize, cts.Token);

IDisplayRenderer renderer = sp.GetRequiredService<IDisplayRenderer>();
await renderer.RunAsync(cts.Token);
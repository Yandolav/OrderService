using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
IConfigurationRoot configuration = manager;

builder
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .Add(new ExternalConfigurationSource());

var baseUri = new Uri(manager["ConfigClient:BaseAddress"] ?? "http://localhost:8080");
int pageSize = int.TryParse(manager["ConfigClient:PageSize"], out int ps) ? ps : 50;
int intervalSec = int.TryParse(manager["ConfigRefresh:IntervalSeconds"], out int isec) ? isec : 3;
string impl = manager["ConfigClient:Implementation"] ?? "Refit";

var services = new ServiceCollection();
services.AddOptions().Configure<DisplayOptions>(manager.GetSection("Display"));
if (string.Equals(impl, "Refit", StringComparison.OrdinalIgnoreCase))
    services.AddConfigClientRefit(baseUri);
else
    services.AddConfigClientHttp(baseUri);
services.AddSingleton(_ => configuration.Providers.OfType<ExternalConfigurationProvider>().Single());
services.AddSingleton<IConfigRefreshService, ConfigRefreshService>();
services.AddSingleton<DisplayRenderer>();

using ServiceProvider sp = services.BuildServiceProvider();

IConfigRefreshService refresh = sp.GetRequiredService<IConfigRefreshService>();
_ = refresh.RunPeriodicRefreshAsync(TimeSpan.FromSeconds(intervalSec), pageSize, cts.Token);

DisplayRenderer renderer = sp.GetRequiredService<DisplayRenderer>();
await renderer.RunAsync(cts.Token);
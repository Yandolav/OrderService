using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Task2.Service;

namespace Presentation.BackgroundServices;

public sealed class ConfigRefreshBackgroundService : BackgroundService
{
    private readonly IConfigRefreshService _refresh;
    private readonly ILogger<ConfigRefreshBackgroundService> _logger;

    public ConfigRefreshBackgroundService(IConfigRefreshService refresh, ILogger<ConfigRefreshBackgroundService> logger)
    {
        _refresh = refresh;
        _logger = logger;
    }

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        _refresh.RefreshOnceAsync(cancellationToken);
        return base.StartAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Background Service is starting");
        try
        {
            await _refresh.RunPeriodicRefreshAsync(stoppingToken);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Background Service is canceled");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while executing MyBackgroundService");
        }
    }
}
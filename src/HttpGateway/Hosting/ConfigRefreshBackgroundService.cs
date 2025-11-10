using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Task2.Service;

namespace HttpGateway.Hosting;

public sealed class ConfigRefreshBackgroundService : BackgroundService
{
    private readonly IConfigRefreshService _refresh;
    private readonly ILogger<ConfigRefreshBackgroundService> _logger;

    public ConfigRefreshBackgroundService(IConfigRefreshService refresh, ILogger<ConfigRefreshBackgroundService> logger)
    {
        _refresh = refresh;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Config refresh background service starting");
        try
        {
            await _refresh.RunPeriodicRefreshAsync(stoppingToken);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Config refresh background service canceled");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Config refresh service error");
        }
    }
}
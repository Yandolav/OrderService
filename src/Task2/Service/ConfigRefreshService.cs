using Microsoft.Extensions.Options;
using Task1.Domain;
using Task2.Provider;

namespace Task2.Service;

public sealed class ConfigRefreshService : IConfigRefreshService
{
    private readonly IConfigClient _client;
    private readonly ExternalConfigurationProvider _provider;
    private readonly IOptions<ConfigRefreshOptions> _options;

    public ConfigRefreshService(IConfigClient client, ExternalConfigurationProvider provider, IOptions<ConfigRefreshOptions> options)
    {
        _client = client;
        _provider = provider;
        _options = options;
    }

    public async Task<bool> RefreshOnceAsync(CancellationToken cancellationToken)
    {
        List<ConfigurationItem> items = await _client.GetAllAsync(cancellationToken).ToListAsync(cancellationToken);
        return _provider.TryApplyItems(items);
    }

    public async Task RunPeriodicRefreshAsync(CancellationToken cancellationToken)
    {
        int intervalSeconds = _options.Value.IntervalSeconds;
        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(intervalSeconds));

        await RefreshOnceAsync(cancellationToken);
        while (await timer.WaitForNextTickAsync(cancellationToken))
        {
            await RefreshOnceAsync(cancellationToken);
        }
    }
}
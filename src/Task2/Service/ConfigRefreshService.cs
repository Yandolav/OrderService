using Task1.Domain;
using Task2.Provider;

namespace Task2.Service;

public sealed class ConfigRefreshService : IConfigRefreshService
{
    private readonly IConfigClient _client;
    private readonly ExternalConfigurationProvider _provider;

    public ConfigRefreshService(IConfigClient client, ExternalConfigurationProvider provider)
    {
        _client = client;
        _provider = provider;
    }

    public async Task<bool> RefreshOnceAsync(int pageSize, CancellationToken ct = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(pageSize);

        IReadOnlyList<ConfigurationItem> items = await _client.GetAllAsync(pageSize, ct);
        return _provider.TryApplyItems(items);
    }

    public async Task RunPeriodicRefreshAsync(TimeSpan interval, int pageSize, CancellationToken ct = default)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(interval, TimeSpan.Zero);

        using var timer = new PeriodicTimer(interval);

        await RefreshOnceAsync(pageSize, ct);
        while (await timer.WaitForNextTickAsync(ct))
        {
            await RefreshOnceAsync(pageSize, ct);
        }
    }
}
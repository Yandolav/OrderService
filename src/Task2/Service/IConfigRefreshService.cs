namespace Task2.Service;

public interface IConfigRefreshService
{
    Task<bool> RefreshOnceAsync(int pageSize, CancellationToken ct = default);

    Task RunPeriodicRefreshAsync(TimeSpan interval, int pageSize, CancellationToken ct = default);
}
namespace Task2.Service;

public interface IConfigRefreshService
{
    Task<bool> RefreshOnceAsync(CancellationToken cancellationToken);

    Task RunPeriodicRefreshAsync(CancellationToken cancellationToken);
}
namespace Core.Application.Ports.SecondaryPorts;

public interface ITransaction : IAsyncDisposable
{
    Task CommitAsync(CancellationToken cancellationToken);
}
namespace Core.Application.Ports.SecondaryPorts;

public interface ITransactionManager
{
    Task<ITransaction> BeginAsync(CancellationToken cancellationToken);
}
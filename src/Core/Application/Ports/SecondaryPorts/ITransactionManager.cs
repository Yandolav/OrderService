namespace Domain.Repositories;

public interface ITransactionManager
{
    Task<ITransaction> BeginAsync(CancellationToken cancellationToken);
}
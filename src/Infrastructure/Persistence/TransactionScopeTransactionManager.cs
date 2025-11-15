using Core.Application.Ports.SecondaryPorts;
using System.Transactions;

namespace Infrastructure.Persistence;

public sealed class TransactionScopeTransactionManager : ITransactionManager
{
    public Task<ITransaction> BeginAsync(CancellationToken cancellationToken)
    {
        var scope = new TransactionScope(
            TransactionScopeOption.Required,
            new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
            TransactionScopeAsyncFlowOption.Enabled);

        ITransaction transaction = new TransactionScopeTransaction(scope);
        return Task.FromResult(transaction);
    }
}
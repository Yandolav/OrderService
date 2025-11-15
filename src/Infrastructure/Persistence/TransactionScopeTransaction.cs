using Core.Application.Ports.SecondaryPorts;
using System.Transactions;

namespace Infrastructure.Persistence;

internal sealed class TransactionScopeTransaction : ITransaction
{
    private readonly TransactionScope _scope;

    public TransactionScopeTransaction(TransactionScope scope)
    {
        _scope = scope;
    }

    public Task CommitAsync(CancellationToken cancellationToken)
    {
        _scope.Complete();
        return Task.CompletedTask;
    }

    public ValueTask DisposeAsync()
    {
        _scope.Dispose();
        return ValueTask.CompletedTask;
    }
}
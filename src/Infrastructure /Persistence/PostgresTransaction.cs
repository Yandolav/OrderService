using Core.Application.Ports.SecondaryPorts;
using Npgsql;

namespace Infrastructure.Persistence;

internal sealed class PostgresTransaction : ITransaction
{
    internal NpgsqlConnection Connection { get; }

    internal NpgsqlTransaction Transaction { get; }

    public PostgresTransaction(NpgsqlConnection connection, NpgsqlTransaction transaction)
    {
        Connection = connection;
        Transaction = transaction;
    }

    public Task CommitAsync(CancellationToken cancellationToken)
    {
        return Transaction.CommitAsync(cancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        await Transaction.DisposeAsync();
        await Connection.DisposeAsync();
    }
}
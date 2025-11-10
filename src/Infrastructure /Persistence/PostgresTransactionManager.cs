using Core.Application.Ports.SecondaryPorts;
using Npgsql;

namespace Infrastructure.Persistence;

public sealed class PostgresTransactionManager : ITransactionManager
{
    private readonly NpgsqlDataSource _dataSource;

    public PostgresTransactionManager(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public async Task<ITransaction> BeginAsync(CancellationToken cancellationToken)
    {
        NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(cancellationToken);
        NpgsqlTransaction transaction = await connection.BeginTransactionAsync(cancellationToken);
        return new PostgresTransaction(connection, transaction);
    }
}
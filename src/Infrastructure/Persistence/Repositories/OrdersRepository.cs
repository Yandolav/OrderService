using Core.Application.Filters;
using Core.Application.Pagination;
using Core.Application.Ports.SecondaryPorts;
using Core.Domain.Entities;
using Core.Domain.Enums;
using Npgsql;
using System.Runtime.CompilerServices;

namespace Infrastructure.Persistence.Repositories;

public sealed class OrdersRepository : IOrdersRepository
{
    private readonly NpgsqlDataSource _dataSource;

    public OrdersRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public async Task<Order?> GetByIdAsync(long orderId, ITransaction? transaction, CancellationToken cancellationToken)
    {
        const string sql = """
                           select order_id, order_state, order_created_at, order_created_by
                           from orders
                           where order_id = :id
                           limit 1;
                           """;

        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(cancellationToken);
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.Add(new NpgsqlParameter("id", orderId));

        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);
        if (await reader.ReadAsync(cancellationToken))
        {
            return new Order(
                reader.GetInt64(0),
                reader.GetFieldValue<OrderState>(1),
                reader.GetFieldValue<DateTimeOffset>(2),
                reader.GetString(3));
        }

        return null;
    }

    public async Task<long> CreateAsync(string createdBy, DateTimeOffset createdAt, ITransaction? transaction, CancellationToken cancellationToken)
    {
        const string sql = """
                           insert into orders(order_state, order_created_at, order_created_by)
                           values ('created', :at, :by) returning order_id;
                           """;

        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(cancellationToken);
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.Add(new NpgsqlParameter("at", createdAt));
        command.Parameters.Add(new NpgsqlParameter("by", createdBy));

        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);
        if (await reader.ReadAsync(cancellationToken))
        {
            return reader.GetInt64(0);
        }

        throw new InvalidOperationException("no rows returned.");
    }

    public async Task<bool> UpdateStateAsync(long orderId, OrderState newState, ITransaction? transaction, CancellationToken cancellationToken)
    {
        const string sql = """
                           update orders set order_state = :state
                           where order_id = :id;
                           """;

        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(cancellationToken);
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.Add(new NpgsqlParameter("id", orderId));
        command.Parameters.Add(new NpgsqlParameter("state", newState));

        int rowsAffected = await command.ExecuteNonQueryAsync(cancellationToken);
        return rowsAffected == 1;
    }

    public async IAsyncEnumerable<Order> SearchAsync(OrderFilter filter, Paging paging, ITransaction? transaction, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        const string sql = """
                           select order_id, order_state, order_created_at, order_created_by
                           from orders
                           where
                               (order_id > :cursor)
                             and (COALESCE(cardinality(:ids), 0) = 0 or order_id = any(:ids))
                             and (:state::order_state is null or order_state = :state)
                             and (:by is null or order_created_by = :by)
                           order by order_id
                           limit :lim;
                           """;

        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(cancellationToken);
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.Add(new NpgsqlParameter("cursor", paging.Cursor));
        command.Parameters.Add(new NpgsqlParameter("ids", filter.Ids));
        object stateValue = filter.State.HasValue ? filter.State.Value : DBNull.Value;
        command.Parameters.Add(new NpgsqlParameter("state", stateValue));
        command.Parameters.Add(new NpgsqlParameter("by", filter.CreatedBy));
        command.Parameters.Add(new NpgsqlParameter("lim", paging.Limit));

        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            yield return new Order(
                reader.GetInt64(0),
                reader.GetFieldValue<OrderState>(1),
                reader.GetFieldValue<DateTimeOffset>(2),
                reader.GetString(3));
        }
    }
}
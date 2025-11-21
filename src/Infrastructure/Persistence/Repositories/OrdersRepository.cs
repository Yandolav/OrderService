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

    public async Task<Order?> GetByIdAsync(long orderId, CancellationToken cancellationToken)
    {
        const string sql = """
                           select order_id, order_state, order_created_at, order_created_by
                           from orders
                           where order_id = :id
                           limit 1;
                           """;

        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(cancellationToken);
        await using var command = new NpgsqlCommand(sql, connection)
        {
            Parameters =
            {
                new NpgsqlParameter("id", orderId),
            },
        };

        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);
        return await reader.ReadAsync(cancellationToken)
            ? new Order(
                reader.GetInt64(0),
                await reader.GetFieldValueAsync<OrderState>(1, cancellationToken),
                await reader.GetFieldValueAsync<DateTimeOffset>(2, cancellationToken),
                reader.GetString(3))
            : null;
    }

    public async Task<long> CreateAsync(string createdBy, DateTimeOffset createdAt, CancellationToken cancellationToken)
    {
        const string sql = """
                           insert into orders(order_state, order_created_at, order_created_by)
                           values ('created', :created_at, :created_by) returning order_id;
                           """;

        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(cancellationToken);
        await using var command = new NpgsqlCommand(sql, connection)
        {
            Parameters =
            {
                new NpgsqlParameter("created_at", createdAt),
                new NpgsqlParameter("created_by", createdBy),
            },
        };

        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);
        return await reader.ReadAsync(cancellationToken) ? reader.GetInt64(0) : throw new InvalidOperationException("no rows returned.");
    }

    public async Task<bool> UpdateStateAsync(long orderId, OrderState newState, CancellationToken cancellationToken)
    {
        const string sql = """
                           update orders set order_state = :state
                           where order_id = :id;
                           """;

        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(cancellationToken);
        await using var command = new NpgsqlCommand(sql, connection)
        {
            Parameters =
            {
                new NpgsqlParameter("id", orderId),
                new NpgsqlParameter("state", newState),
            },
        };

        int rowsAffected = await command.ExecuteNonQueryAsync(cancellationToken);
        return rowsAffected == 1;
    }

    public async IAsyncEnumerable<Order> SearchAsync(OrderFilter filter, Paging paging, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        const string sql = """
                           select order_id, order_state, order_created_at, order_created_by
                           from orders
                           where
                               (order_id > :cursor)
                             and (cardinality(:order_ids) = 0 or order_id = any(:order_ids))
                             and (:state::order_state is null or order_state = :state)
                             and (:created_by is null or order_created_by = :created_by)
                           order by order_id
                           limit :limit;
                           """;

        object stateValue = filter.State.HasValue ? filter.State.Value : DBNull.Value;
        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(cancellationToken);
        await using var command = new NpgsqlCommand(sql, connection)
        {
            Parameters =
            {
                new NpgsqlParameter("cursor", paging.Cursor),
                new NpgsqlParameter("order_ids", filter.Ids),
                new NpgsqlParameter("state", stateValue),
                new NpgsqlParameter("created_by", filter.CreatedBy),
                new NpgsqlParameter("limit", paging.Limit),
            },
        };

        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            yield return new Order(
                reader.GetInt64(0),
                await reader.GetFieldValueAsync<OrderState>(1, cancellationToken),
                await reader.GetFieldValueAsync<DateTimeOffset>(2, cancellationToken),
                reader.GetString(3));
        }
    }
}
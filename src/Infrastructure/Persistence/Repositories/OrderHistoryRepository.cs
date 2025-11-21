using Core.Application.Filters;
using Core.Application.Pagination;
using Core.Application.Ports.SecondaryPorts;
using Core.Domain.Entities;
using Core.Domain.Enums;
using Core.Domain.Payloads;
using Npgsql;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace Infrastructure.Persistence.Repositories;

public sealed class OrderHistoryRepository : IOrderHistoryRepository
{
    private readonly NpgsqlDataSource _dataSource;

    public OrderHistoryRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public async Task<long> CreateAsync(long orderId, DateTimeOffset createdAt, OrderHistoryItemKind kind, IOrderHistoryPayload payload, CancellationToken cancellationToken)
    {
        const string sql = """
                           insert into order_history(order_id, order_history_item_created_at, order_history_item_kind, order_history_item_payload)
                           values (:order_id, :created_at, :kind, :payload::jsonb)
                           returning order_history_item_id;
                           """;

        string json = JsonSerializer.Serialize(payload);
        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(cancellationToken);
        await using var command = new NpgsqlCommand(sql, connection)
        {
            Parameters =
            {
                new NpgsqlParameter("order_id", orderId),
                new NpgsqlParameter("created_at", createdAt),
                new NpgsqlParameter("kind", kind),
                new NpgsqlParameter("payload", json),
            },
        };

        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);
        return await reader.ReadAsync(cancellationToken) ? reader.GetInt64(0) : throw new InvalidOperationException("no rows returned.");
    }

    public async IAsyncEnumerable<OrderHistory> SearchAsync(OrderHistoryFilter filter, Paging paging, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        const string sql = """
                           select order_history_item_id, order_id, order_history_item_created_at, order_history_item_kind, order_history_item_payload
                           from order_history
                           where 
                               (order_history_item_id > :cursor)
                             and (cardinality(:order_ids) = 0 or order_id = any(:order_ids))
                             and (:kind::order_history_item_kind is null or order_history_item_kind = :kind)
                           order by order_history_item_id
                           limit :limit;
                           """;

        object kindValue = filter.Kind.HasValue ? filter.Kind.Value : DBNull.Value;
        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(cancellationToken);
        await using var command = new NpgsqlCommand(sql, connection)
        {
            Parameters =
            {
                new NpgsqlParameter("cursor", paging.Cursor),
                new NpgsqlParameter("order_ids", filter.OrderIds),
                new NpgsqlParameter("kind", kindValue),
                new NpgsqlParameter("limit", paging.Limit),
            },
        };

        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            string json = reader.GetString(4);
            IOrderHistoryPayload payload = JsonSerializer.Deserialize<IOrderHistoryPayload>(json) ?? throw new InvalidOperationException();

            yield return new OrderHistory(
                reader.GetInt64(0),
                reader.GetInt64(1),
                await reader.GetFieldValueAsync<DateTimeOffset>(2, cancellationToken),
                await reader.GetFieldValueAsync<OrderHistoryItemKind>(3, cancellationToken),
                payload);
        }
    }
}
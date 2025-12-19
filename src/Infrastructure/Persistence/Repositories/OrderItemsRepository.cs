using Core.Abstractions.Repositories;
using Core.Model.Entities;
using Core.Model.Filters;
using Core.Model.Pagination;
using Npgsql;
using System.Runtime.CompilerServices;

namespace Infrastructure.Persistence.Repositories;

public sealed class OrderItemsRepository : IOrderItemsRepository
{
    private readonly NpgsqlDataSource _dataSource;

    public OrderItemsRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public async Task<OrderItem?> GetByIdAsync(long orderItemId, CancellationToken cancellationToken)
    {
        const string sql = """
                           select order_item_id, order_id, product_id, order_item_quantity, order_item_deleted
                           from order_items
                           where order_item_id = :id;
                           """;

        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(cancellationToken);
        await using var command = new NpgsqlCommand(sql, connection)
        {
            Parameters =
            {
                new NpgsqlParameter("id", orderItemId),
            },
        };

        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);
        return await reader.ReadAsync(cancellationToken)
            ? new OrderItem(
                reader.GetInt64(0),
                reader.GetInt64(1),
                reader.GetInt64(2),
                reader.GetInt32(3),
                reader.GetBoolean(4))
            : null;
    }

    public async Task<long> CreateAsync(long orderId, long productId, int quantity, CancellationToken cancellationToken)
    {
        const string sql = """
                           insert into order_items(order_id, product_id, order_item_quantity, order_item_deleted)
                           values (:order_id, :product_id, :quantity, false) 
                           returning order_item_id;
                           """;

        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(cancellationToken);
        await using var command = new NpgsqlCommand(sql, connection)
        {
            Parameters =
            {
                new NpgsqlParameter("order_id", orderId),
                new NpgsqlParameter("product_id", productId),
                new NpgsqlParameter("quantity", quantity),
            },
        };

        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);
        return await reader.ReadAsync(cancellationToken) ? reader.GetInt64(0) : throw new InvalidOperationException("no rows returned.");
    }

    public async Task<bool> SoftDeleteAsync(long orderItemId, CancellationToken cancellationToken)
    {
        const string sql = """
                           update order_items set order_item_deleted = true 
                           where order_item_id = :id;
                           """;

        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(cancellationToken);
        await using var command = new NpgsqlCommand(sql, connection)
        {
            Parameters =
            {
                new NpgsqlParameter("id", orderItemId),
            },
        };

        int rowsAffected = await command.ExecuteNonQueryAsync(cancellationToken);
        return rowsAffected == 1;
    }

    public async IAsyncEnumerable<OrderItem> SearchAsync(OrderItemsFilter filter, Paging paging, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        const string sql = """
                           select order_item_id, order_id, product_id, order_item_quantity, order_item_deleted
                           from order_items
                           where
                               (order_item_id > :cursor)
                             and (cardinality(:order_ids) = 0 or order_id = any(:order_ids))
                             and (cardinality(:product_ids) = 0 or product_id = any(:product_ids))
                             and (:deleted is null or order_item_deleted = :deleted)
                           order by order_item_id
                           limit :limit;
                           """;

        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(cancellationToken);
        await using var command = new NpgsqlCommand(sql, connection)
        {
            Parameters =
            {
                new NpgsqlParameter("cursor", paging.Cursor),
                new NpgsqlParameter("order_ids", filter.OrderIds),
                new NpgsqlParameter("product_ids", filter.ProductIds),
                new NpgsqlParameter("deleted", filter.Deleted),
                new NpgsqlParameter("limit", paging.Limit),
            },
        };

        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            yield return new OrderItem(
                reader.GetInt64(0),
                reader.GetInt64(1),
                reader.GetInt64(2),
                reader.GetInt32(3),
                reader.GetBoolean(4));
        }
    }
}
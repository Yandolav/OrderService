using Domain.Entities;
using Domain.Entities.Filters;
using Domain.Entities.Pagination;
using Domain.Repositories;
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

    public async Task<OrderItem?> GetByIdAsync(long orderItemId, ITransaction? transaction, CancellationToken cancellationToken)
    {
        const string sql = """
                           select order_item_id, order_id, product_id, order_item_quantity, order_item_deleted
                           from order_items
                           where order_item_id = :id;
                           """;

        await using NpgsqlCommand command = transaction is PostgresTransaction postgresTransaction ? new NpgsqlCommand(sql, postgresTransaction.Connection, postgresTransaction.Transaction) : _dataSource.CreateCommand(sql);
        command.Parameters.Add(new NpgsqlParameter("id", orderItemId));

        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);
        if (await reader.ReadAsync(cancellationToken))
        {
            return new OrderItem(
                reader.GetInt64(0),
                reader.GetInt64(1),
                reader.GetInt64(2),
                reader.GetInt32(3),
                reader.GetBoolean(4));
        }

        return null;
    }

    public async Task<long> CreateAsync(long orderId, long productId, int quantity, ITransaction? transaction, CancellationToken cancellationToken)
    {
        const string sql = """
                           insert into order_items(order_id, product_id, order_item_quantity, order_item_deleted)
                           values (:oid, :pid, :quantity, false) 
                           returning order_item_id;
                           """;

        await using NpgsqlCommand command = transaction is PostgresTransaction postgresTransaction ? new NpgsqlCommand(sql, postgresTransaction.Connection, postgresTransaction.Transaction) : _dataSource.CreateCommand(sql);
        command.Parameters.Add(new NpgsqlParameter("oid", orderId));
        command.Parameters.Add(new NpgsqlParameter("pid", productId));
        command.Parameters.Add(new NpgsqlParameter("quantity", quantity));

        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);
        if (await reader.ReadAsync(cancellationToken))
        {
            return reader.GetInt64(0);
        }

        throw new InvalidOperationException("no rows returned.");
    }

    public async Task<bool> SoftDeleteAsync(long orderItemId, ITransaction? transaction, CancellationToken cancellationToken)
    {
        const string sql = """
                           update order_items set order_item_deleted = true 
                           where order_item_id = :id;
                           """;

        await using NpgsqlCommand command = transaction is PostgresTransaction postgresTransaction ? new NpgsqlCommand(sql, postgresTransaction.Connection, postgresTransaction.Transaction) : _dataSource.CreateCommand(sql);
        command.Parameters.Add(new NpgsqlParameter("id", orderItemId));

        int rowsAffected = await command.ExecuteNonQueryAsync(cancellationToken);
        return rowsAffected == 1;
    }

    public async IAsyncEnumerable<OrderItem> SearchAsync(OrderItemsFilter filter, Paging paging, ITransaction? transaction, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        const string sql = """
                           select order_item_id, order_id, product_id, order_item_quantity, order_item_deleted
                           from order_items
                           where
                               (order_item_id > :cursor)
                             and (COALESCE(cardinality(:oids), 0) = 0 or order_id = any(:oids))
                             and (COALESCE(cardinality(:pids), 0) = 0 or product_id = any(:pids))
                             and (:del is null or order_item_deleted = :del)
                           order by order_item_id
                           limit :lim;
                           """;

        await using NpgsqlCommand command = transaction is PostgresTransaction postgresTransaction ? new NpgsqlCommand(sql, postgresTransaction.Connection, postgresTransaction.Transaction) : _dataSource.CreateCommand(sql);
        command.Parameters.Add(new NpgsqlParameter("cursor", paging.Cursor));
        command.Parameters.Add(new NpgsqlParameter("oids", filter.OrderIds));
        command.Parameters.Add(new NpgsqlParameter("pids", filter.ProductIds));
        command.Parameters.Add(new NpgsqlParameter("del", filter.Deleted));
        command.Parameters.Add(new NpgsqlParameter("lim", paging.Limit));

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
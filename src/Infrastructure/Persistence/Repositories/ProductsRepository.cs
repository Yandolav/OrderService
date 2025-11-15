using Core.Application.Filters;
using Core.Application.Pagination;
using Core.Application.Ports.SecondaryPorts;
using Core.Domain.Entities;
using Npgsql;
using System.Runtime.CompilerServices;

namespace Infrastructure.Persistence.Repositories;

public sealed class ProductsRepository : IProductsRepository
{
    private readonly NpgsqlDataSource _dataSource;

    public ProductsRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public async Task<long> CreateAsync(string name, decimal price, CancellationToken cancellationToken)
    {
        const string sql = """
                           insert into products(product_name, product_price)
                           values (:name, :price) 
                           returning product_id;
                           """;

        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(cancellationToken);
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.Add(new NpgsqlParameter("name", name));
        command.Parameters.Add(new NpgsqlParameter("price", price));

        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);
        if (await reader.ReadAsync(cancellationToken))
        {
            return reader.GetInt64(0);
        }

        throw new InvalidOperationException("no rows returned.");
    }

    public async IAsyncEnumerable<Product> SearchAsync(ProductFilter filter, Paging paging, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        const string sql = """
                           select product_id, product_name, product_price
                           from products
                           where 
                               (product_id > :cursor)
                             and (COALESCE(cardinality(:ids), 0) = 0 or product_id = any(:ids))
                             and (:minimum_price is null or product_price >= :minimum_price)
                             and (:maximum_price is null or product_price <= :maximum_price)
                             and (:name_pattern is null or product_name like :name_pattern)
                           order by product_id
                           limit :limit;
                           """;

        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(cancellationToken);
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.Add(new NpgsqlParameter("ids", filter.Ids));
        command.Parameters.Add(new NpgsqlParameter("minimum_price", filter.MinPrice));
        command.Parameters.Add(new NpgsqlParameter("maximum_price", filter.MaxPrice));
        command.Parameters.Add(new NpgsqlParameter("name_pattern", filter.NameContains));
        command.Parameters.Add(new NpgsqlParameter("cursor", paging.Cursor));
        command.Parameters.Add(new NpgsqlParameter("limit", paging.Limit));

        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            yield return new Product(reader.GetInt64(0), reader.GetString(1), reader.GetFieldValue<decimal>(2));
        }
    }
}
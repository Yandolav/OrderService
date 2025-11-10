using Domain.Entities;
using Domain.Entities.Filters;
using Domain.Entities.Pagination;

namespace Domain.Repositories;

public interface IProductsRepository
{
    Task<long> CreateAsync(string name, decimal price, ITransaction? transaction, CancellationToken cancellationToken);

    IAsyncEnumerable<Product> SearchAsync(ProductFilter filter, Paging paging, ITransaction? transaction, CancellationToken cancellationToken);
}
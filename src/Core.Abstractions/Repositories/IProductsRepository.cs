using Core.Model.Entities;
using Core.Model.Filters;
using Core.Model.Pagination;

namespace Core.Abstractions.Repositories;

public interface IProductsRepository
{
    Task<long> CreateAsync(string name, decimal price, CancellationToken cancellationToken);

    IAsyncEnumerable<Product> SearchAsync(ProductFilter filter, Paging paging, CancellationToken cancellationToken);
}
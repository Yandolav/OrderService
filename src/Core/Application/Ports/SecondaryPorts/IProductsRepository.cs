using Core.Application.Filters;
using Core.Application.Pagination;
using Core.Domain.Entities;

namespace Core.Application.Ports.SecondaryPorts;

public interface IProductsRepository
{
    Task<long> CreateAsync(string name, decimal price, ITransaction? transaction, CancellationToken cancellationToken);

    IAsyncEnumerable<Product> SearchAsync(ProductFilter filter, Paging paging, ITransaction? transaction, CancellationToken cancellationToken);
}
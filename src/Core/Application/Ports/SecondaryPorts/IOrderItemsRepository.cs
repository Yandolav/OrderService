using Domain.Entities;
using Domain.Entities.Filters;
using Domain.Entities.Pagination;

namespace Domain.Repositories;

public interface IOrderItemsRepository
{
    Task<OrderItem?> GetByIdAsync(long orderItemId, ITransaction? transaction, CancellationToken cancellationToken);

    Task<long> CreateAsync(long orderId, long productId, int quantity, ITransaction? transaction, CancellationToken cancellationToken);

    Task<bool> SoftDeleteAsync(long orderItemId, ITransaction? transaction, CancellationToken cancellationToken);

    IAsyncEnumerable<OrderItem> SearchAsync(OrderItemsFilter filter, Paging paging, ITransaction? transaction, CancellationToken cancellationToken);
}
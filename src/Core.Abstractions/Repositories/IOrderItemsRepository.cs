using Core.Model.Entities;
using Core.Model.Filters;
using Core.Model.Pagination;

namespace Core.Abstractions.Repositories;

public interface IOrderItemsRepository
{
    Task<OrderItem?> GetByIdAsync(long orderItemId, CancellationToken cancellationToken);

    Task<long> CreateAsync(long orderId, long productId, int quantity, CancellationToken cancellationToken);

    Task<bool> SoftDeleteAsync(long orderItemId, CancellationToken cancellationToken);

    IAsyncEnumerable<OrderItem> SearchAsync(OrderItemsFilter filter, Paging paging, CancellationToken cancellationToken);
}
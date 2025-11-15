using Core.Application.Filters;
using Core.Application.Pagination;
using Core.Domain.Entities;

namespace Core.Application.Ports.SecondaryPorts;

public interface IOrderItemsRepository
{
    Task<OrderItem?> GetByIdAsync(long orderItemId, CancellationToken cancellationToken);

    Task<long> CreateAsync(long orderId, long productId, int quantity, CancellationToken cancellationToken);

    Task<bool> SoftDeleteAsync(long orderItemId, CancellationToken cancellationToken);

    IAsyncEnumerable<OrderItem> SearchAsync(OrderItemsFilter filter, Paging paging, CancellationToken cancellationToken);
}
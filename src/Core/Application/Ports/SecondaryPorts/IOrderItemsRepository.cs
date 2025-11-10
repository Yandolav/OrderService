using Core.Application.Filters;
using Core.Application.Pagination;
using Core.Domain.Entities;

namespace Core.Application.Ports.SecondaryPorts;

public interface IOrderItemsRepository
{
    Task<OrderItem?> GetByIdAsync(long orderItemId, ITransaction? transaction, CancellationToken cancellationToken);

    Task<long> CreateAsync(long orderId, long productId, int quantity, ITransaction? transaction, CancellationToken cancellationToken);

    Task<bool> SoftDeleteAsync(long orderItemId, ITransaction? transaction, CancellationToken cancellationToken);

    IAsyncEnumerable<OrderItem> SearchAsync(OrderItemsFilter filter, Paging paging, ITransaction? transaction, CancellationToken cancellationToken);
}
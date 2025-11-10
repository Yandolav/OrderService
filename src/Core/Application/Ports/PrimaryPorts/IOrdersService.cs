using Domain.Entities;
using Domain.Entities.Pagination;
using Domain.Enums;

namespace Application.ServiceInterfaces;

public interface IOrdersService
{
    Task<long> CreateOrderAsync(string createdBy, CancellationToken cancellationToken);

    Task<long> AddOrderItemAsync(long orderId, long productId, int quantity, CancellationToken cancellationToken);

    Task<bool> RemoveOrderItemAsync(long orderItemId, CancellationToken cancellationToken);

    Task<bool> StartProcessingAsync(long orderId, CancellationToken cancellationToken);

    Task<bool> CompleteAsync(long orderId, CancellationToken cancellationToken);

    Task<bool> CancelAsync(long orderId, CancellationToken cancellationToken);

    IAsyncEnumerable<OrderHistory> GetOrderHistoryAsync(long[]? orderIds, OrderHistoryItemKind? kind, Paging paging, CancellationToken cancellationToken);
}
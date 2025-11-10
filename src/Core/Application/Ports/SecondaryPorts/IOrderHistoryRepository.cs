using Domain.Entities;
using Domain.Entities.Filters;
using Domain.Entities.Pagination;
using Domain.Entities.Payloads;
using Domain.Enums;

namespace Domain.Repositories;

public interface IOrderHistoryRepository
{
    Task<long> CreateAsync(long orderId, DateTimeOffset createdAt, OrderHistoryItemKind kind, IOrderHistoryPayload payload, ITransaction? transaction, CancellationToken cancellationToken);

    IAsyncEnumerable<OrderHistory> SearchAsync(OrderHistoryFilter filter, Paging paging, ITransaction? transaction, CancellationToken cancellationToken);
}
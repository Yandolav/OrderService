using Core.Model.Entities;
using Core.Model.Enums;
using Core.Model.Filters;
using Core.Model.Pagination;
using Core.Model.Payloads;

namespace Core.Abstractions.Repositories;

public interface IOrderHistoryRepository
{
    Task<long> CreateAsync(long orderId, DateTimeOffset createdAt, OrderHistoryItemKind kind, IOrderHistoryPayload payload, CancellationToken cancellationToken);

    IAsyncEnumerable<OrderHistory> SearchAsync(OrderHistoryFilter filter, Paging paging, CancellationToken cancellationToken);
}
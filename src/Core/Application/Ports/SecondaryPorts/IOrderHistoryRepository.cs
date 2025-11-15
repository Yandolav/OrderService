using Core.Application.Filters;
using Core.Application.Pagination;
using Core.Domain.Entities;
using Core.Domain.Enums;
using Core.Domain.Payloads;

namespace Core.Application.Ports.SecondaryPorts;

public interface IOrderHistoryRepository
{
    Task<long> CreateAsync(long orderId, DateTimeOffset createdAt, OrderHistoryItemKind kind, IOrderHistoryPayload payload, CancellationToken cancellationToken);

    IAsyncEnumerable<OrderHistory> SearchAsync(OrderHistoryFilter filter, Paging paging, CancellationToken cancellationToken);
}
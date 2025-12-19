using Core.Model.Enums;
using Core.Model.Payloads;

namespace Core.Contracts.OrderHistory;

public interface IOrderHistoryService
{
    Task CreateAsync(long orderId, OrderHistoryItemKind kind, IOrderHistoryPayload payload, CancellationToken cancellationToken);
}
using Core.Domain.Enums;
using Core.Domain.Payloads;

namespace Core.Application.Ports.PrimaryPorts;

public interface IOrderHistoryService
{
    Task CreateAsync(long orderId, OrderHistoryItemKind kind, IOrderHistoryPayload payload, CancellationToken cancellationToken);
}
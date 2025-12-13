using Core.Application.Ports.PrimaryPorts;
using Core.Application.Ports.SecondaryPorts;
using Core.Domain.Enums;
using Core.Domain.Payloads;

namespace Core.Application.Services;

public class OrderHistoryService : IOrderHistoryService
{
    private readonly IOrderHistoryRepository _historyRepository;
    private readonly TimeProvider _timeProvider;

    public OrderHistoryService(IOrderHistoryRepository historyRepository, TimeProvider timeProvider)
    {
        _historyRepository = historyRepository;
        _timeProvider = timeProvider;
    }

    public Task CreateAsync(long orderId, OrderHistoryItemKind kind, IOrderHistoryPayload payload, CancellationToken cancellationToken)
    {
        DateTimeOffset now = _timeProvider.GetUtcNow();
        return _historyRepository.CreateAsync(orderId, now, kind, payload, cancellationToken);
    }
}
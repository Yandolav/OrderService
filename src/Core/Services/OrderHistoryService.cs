using Core.Abstractions.Repositories;
using Core.Contracts.OrderHistory;
using Core.Model.Enums;
using Core.Model.Payloads;

namespace Core.Services;

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
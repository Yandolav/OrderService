using Core.Model.Entities;

namespace Core.Contracts.Events;

public interface IOrderEventsProducer
{
    Task OrderCreatedAsync(Order order, CancellationToken cancellationToken);

    Task OrderProcessingStartedAsync(Order order, CancellationToken cancellationToken);
}
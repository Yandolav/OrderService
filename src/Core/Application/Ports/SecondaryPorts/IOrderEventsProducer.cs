using Core.Domain.Entities;

namespace Core.Application.Ports.SecondaryPorts;

public interface IOrderEventsProducer
{
    Task OrderCreatedAsync(Order order, CancellationToken cancellationToken);

    Task OrderProcessingStartedAsync(Order order, CancellationToken cancellationToken);
}
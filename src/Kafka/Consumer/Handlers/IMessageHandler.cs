using Confluent.Kafka;
using Orders.Kafka.Contracts;

namespace Kafka.Consumer.Handlers;

public interface IMessageHandler
{
    Task HandleASync(IEnumerable<ConsumeResult<OrderProcessingKey, OrderProcessingValue>> messages, CancellationToken cancellationToken);
}
using Core.Application.Ports.SecondaryPorts;
using Core.Domain.Entities;
using Google.Protobuf.WellKnownTypes;
using Kafka.Options;
using Microsoft.Extensions.Options;
using Orders.Kafka.Contracts;

namespace Kafka.Producer;

public sealed class OrderEventsProducer : IOrderEventsProducer
{
    private readonly IKafkaProducer<OrderCreationKey, OrderCreationValue> _producer;
    private readonly KafkaTopicsOptions _topics;
    private readonly TimeProvider _timeProvider;

    public OrderEventsProducer(
        IKafkaProducer<OrderCreationKey, OrderCreationValue> producer,
        IOptions<KafkaTopicsOptions> topicsOptions,
        TimeProvider timeProvider)
    {
        _producer = producer;
        _topics = topicsOptions.Value;
        _timeProvider = timeProvider;
    }

    public Task OrderCreatedAsync(Order order, CancellationToken cancellationToken)
    {
        var key = new OrderCreationKey
        {
            OrderId = order.OrderId,
        };

        DateTimeOffset now = _timeProvider.GetUtcNow();

        var value = new OrderCreationValue
        {
            OrderCreated = new OrderCreationValue.Types.OrderCreated
            {
                OrderId = order.OrderId,
                CreatedAt = Timestamp.FromDateTime(now.UtcDateTime),
            },
        };

        string topic = _topics.OrderCreationTopic ?? throw new InvalidOperationException("Kafka topic 'OrderCreationTopic' is not configured.");

        return _producer.ProduceAsync(topic, key, value, cancellationToken);
    }

    public Task OrderProcessingStartedAsync(Order order, CancellationToken cancellationToken)
    {
        var key = new OrderCreationKey
        {
            OrderId = order.OrderId,
        };

        DateTimeOffset now = _timeProvider.GetUtcNow();

        var value = new OrderCreationValue
        {
            OrderProcessingStarted = new OrderCreationValue.Types.OrderProcessingStarted
            {
                OrderId = order.OrderId,
                StartedAt = Timestamp.FromDateTime(now.UtcDateTime),
            },
        };

        string topic = _topics.OrderCreationTopic ?? throw new InvalidOperationException("Kafka topic 'OrderCreationTopic' is not configured.");

        return _producer.ProduceAsync(topic, key, value, cancellationToken);
    }
}
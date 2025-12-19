using Core.Contracts.Events;
using Core.Model.Entities;
using Google.Protobuf.WellKnownTypes;
using Kafka.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Orders.Kafka.Contracts;

namespace Kafka.Producer;

public sealed class OrderEventsProducer : IOrderEventsProducer
{
    private readonly IKafkaProducer<OrderCreationKey, OrderCreationValue> _mainProducer;
    private readonly IKafkaProducer<OrderCreationKey, OrderCreationValue> _otherProducer;
    private readonly KafkaTopicsOptions _topics;
    private readonly TimeProvider _timeProvider;

    public OrderEventsProducer(
        [FromKeyedServices("MainProducer")] IKafkaProducer<OrderCreationKey, OrderCreationValue> mainProducer,
        [FromKeyedServices("OtherProducer")] IKafkaProducer<OrderCreationKey, OrderCreationValue> otherProducer,
        IOptions<KafkaTopicsOptions> topicsOptions,
        TimeProvider timeProvider)
    {
        _mainProducer = mainProducer;
        _otherProducer = otherProducer;
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

        return _mainProducer.ProduceAsync(topic, key, value, cancellationToken);
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

        return _otherProducer.ProduceAsync(topic, key, value, cancellationToken);
    }
}
using Confluent.Kafka;
using Kafka.Options;
using Microsoft.Extensions.Options;

namespace Kafka.Consumer;

public class KafkaConsumer<TKey, TValue> : IKafkaConsumer<TKey, TValue>, IDisposable
{
    private readonly IConsumer<TKey, TValue> _consumer;

    public KafkaConsumer(
        IOptions<KafkaTopicsOptions> topicsOptions,
        IOptions<KafkaConsumerOptions> kafkaConsumerOptions,
        IDeserializer<TKey> keyDeserializer,
        IDeserializer<TValue> valueDeserializer)
    {
        KafkaTopicsOptions topicsOptionsValue = topicsOptions.Value;
        KafkaConsumerOptions kafkaConsumerOptionsValue = kafkaConsumerOptions.Value;

        var config = new ConsumerConfig
        {
            BootstrapServers = kafkaConsumerOptionsValue.BootstrapServers,
            GroupId = kafkaConsumerOptionsValue.GroupId,
            GroupInstanceId = kafkaConsumerOptionsValue.GroupInstanceId,
            EnableAutoCommit = kafkaConsumerOptionsValue.EnableAutoCommit,
            AutoOffsetReset = AutoOffsetReset.Earliest,
        };

        _consumer = new ConsumerBuilder<TKey, TValue>(config)
            .SetKeyDeserializer(keyDeserializer)
            .SetValueDeserializer(valueDeserializer)
            .Build();
        _consumer.Subscribe(topicsOptionsValue.OrderProcessingTopic);
    }

    public ConsumeResult<TKey, TValue> Consumer(CancellationToken cancellationToken)
    {
        return _consumer.Consume(cancellationToken);
    }

    public void Commit(ConsumeResult<TKey, TValue> result)
    {
        _consumer.Commit(result);
    }

    public void Dispose()
    {
        _consumer.Dispose();
    }
}
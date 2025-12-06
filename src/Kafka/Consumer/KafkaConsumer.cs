using Confluent.Kafka;
using Kafka.Options;
using Microsoft.Extensions.Options;

namespace Kafka.Consumer;

public class KafkaConsumer<TKey, TValue> : IKafkaConsumer<TKey, TValue>, IDisposable
{
    private readonly IConsumer<TKey, TValue> _consumer;

    public KafkaConsumer(
        IOptions<KafkaTopicsOptions> topicsOptions,
        IOptions<KafkaOptions> kafkaOptions,
        IOptions<KafkaConsumerOptions> kafkaConsumerOptions,
        IDeserializer<TKey> keyDeserializer,
        IDeserializer<TValue> valueDeserializer)
    {
        KafkaTopicsOptions topicsOptionsValue = topicsOptions.Value;
        KafkaOptions kafkaOptionsValue = kafkaOptions.Value;
        KafkaConsumerOptions kafkaConsumerOptionsValue = kafkaConsumerOptions.Value;

        var config = new ConsumerConfig
        {
            BootstrapServers = kafkaOptionsValue.BootstrapServers,
            GroupId = kafkaConsumerOptionsValue.GroupId,
            GroupInstanceId = kafkaConsumerOptionsValue.FirstInstance,
            EnableAutoCommit = false,
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

    public void Commit()
    {
        _consumer.Commit();
    }

    public void Dispose()
    {
        _consumer.Dispose();
    }
}
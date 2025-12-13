using Confluent.Kafka;
using Kafka.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Kafka.Producer;

public sealed class KafkaProducer<TKey, TValue> : IKafkaProducer<TKey, TValue>, IDisposable
{
    private readonly IProducer<TKey, TValue> _producer;
    private readonly ILogger<KafkaProducer<TKey, TValue>> _logger;

    public KafkaProducer(
        IOptions<KafkaProducerOptions> kafkaOptions,
        ISerializer<TKey> keySerializer,
        ISerializer<TValue> valueSerializer,
        ILogger<KafkaProducer<TKey, TValue>> logger)
    {
        _logger = logger;
        KafkaProducerOptions producerOptions = kafkaOptions.Value;

        var config = new ProducerConfig
        {
            BootstrapServers = producerOptions.BootstrapServers,
            ClientId = producerOptions.ClientId,
            Acks = Acks.All,
        };

        _producer = new ProducerBuilder<TKey, TValue>(config)
            .SetKeySerializer(keySerializer)
            .SetValueSerializer(valueSerializer)
            .Build();
    }

    public async Task ProduceAsync(
        string topic,
        TKey key,
        TValue value,
        CancellationToken cancellationToken)
    {
        var message = new Message<TKey, TValue>
        {
            Key = key,
            Value = value,
        };

        await _producer.ProduceAsync(topic, message, cancellationToken);
    }

    public void Dispose()
    {
        try
        {
            _producer.Flush(TimeSpan.FromSeconds(5));
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error while flushing Kafka producer during dispose.");
        }

        _producer.Dispose();
    }
}
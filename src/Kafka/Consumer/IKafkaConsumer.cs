using Confluent.Kafka;

namespace Kafka.Consumer;

public interface IKafkaConsumer<TKey, TValue>
{
    ConsumeResult<TKey, TValue> Consumer(CancellationToken cancellationToken);

    void Commit(ConsumeResult<TKey, TValue> result);
}
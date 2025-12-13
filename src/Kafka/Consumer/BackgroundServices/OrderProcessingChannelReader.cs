using Confluent.Kafka;
using Orders.Kafka.Contracts;
using System.Threading.Channels;

namespace Kafka.Consumer.BackgroundServices;

internal sealed class OrderProcessingChannelReader
{
    private readonly IKafkaConsumer<OrderProcessingKey, OrderProcessingValue> _consumer;

    public OrderProcessingChannelReader(IKafkaConsumer<OrderProcessingKey, OrderProcessingValue> consumer)
    {
        _consumer = consumer;
    }

    public async Task ReadAsync(
        ChannelWriter<ConsumeResult<OrderProcessingKey, OrderProcessingValue>> writer,
        CancellationToken cancellationToken)
    {
        await Task.Yield();

        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                ConsumeResult<OrderProcessingKey, OrderProcessingValue> message = _consumer.Consumer(cancellationToken);
                await writer.WriteAsync(message, cancellationToken);
            }
        }
        finally
        {
            writer.TryComplete();
        }
    }
}
using Confluent.Kafka;
using Kafka.Consumer.Handlers;
using Microsoft.Extensions.DependencyInjection;
using Orders.Kafka.Contracts;
using System.Threading.Channels;

namespace Kafka.Consumer.BackgroundServices;

internal sealed class OrderProcessingChannelBatchHandler
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IKafkaConsumer<OrderProcessingKey, OrderProcessingValue> _consumer;

    private readonly int _maxBatchSize;
    private readonly TimeSpan _maxWait;

    public OrderProcessingChannelBatchHandler(
        IServiceScopeFactory scopeFactory,
        IKafkaConsumer<OrderProcessingKey, OrderProcessingValue> consumer,
        int maxBatchSize,
        TimeSpan maxWait)
    {
        _scopeFactory = scopeFactory;
        _consumer = consumer;
        _maxBatchSize = maxBatchSize;
        _maxWait = maxWait;
    }

    public async Task HandleAsync(
        ChannelReader<ConsumeResult<OrderProcessingKey, OrderProcessingValue>> reader,
        CancellationToken cancellationToken)
    {
        await Task.Yield();

        await foreach (IReadOnlyList<ConsumeResult<OrderProcessingKey, OrderProcessingValue>> batch in reader.ReadAllAsync(cancellationToken).ChunkBySizeOrInactivityTimeout(_maxBatchSize, _maxWait, TimeSpan.FromMilliseconds(50), cancellationToken))
        {
            await FlushAsync(batch, cancellationToken);
        }
    }

    private async Task FlushAsync(
        IReadOnlyList<ConsumeResult<OrderProcessingKey, OrderProcessingValue>> batch,
        CancellationToken cancellationToken)
    {
        using IServiceScope scope = _scopeFactory.CreateScope();
        IMessageHandler handler = scope.ServiceProvider.GetRequiredService<IMessageHandler>();

        await handler.HandleASync(batch, cancellationToken);

        foreach (ConsumeResult<OrderProcessingKey, OrderProcessingValue> last in batch.GroupBy(x => x.TopicPartition).Select(g => g.Last()))
        {
            _consumer.Commit(last);
        }
    }
}
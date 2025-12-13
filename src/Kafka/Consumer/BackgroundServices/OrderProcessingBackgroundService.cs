using Confluent.Kafka;
using Kafka.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Orders.Kafka.Contracts;
using System.Threading.Channels;

namespace Kafka.Consumer.BackgroundServices;

public class OrderProcessingBackgroundService : BackgroundService
{
    private readonly IOptionsMonitor<BackgroundServiceOptions> _backgroundOptions;
    private readonly IOptions<KafkaChannelOptions> _kafkaChannelOptions;
    private readonly IServiceScopeFactory _scopeFactory;

    public OrderProcessingBackgroundService(
        IOptionsMonitor<BackgroundServiceOptions> backgroundOptions,
        IOptions<KafkaChannelOptions> kafkaChannelOptions,
        IServiceScopeFactory scopeFactory)
    {
        _backgroundOptions = backgroundOptions;
        _kafkaChannelOptions = kafkaChannelOptions;
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Task t1 = RunPipelineAsync(KafkaOptionsName.MainConsumer, stoppingToken);
        Task t2 = RunPipelineAsync(KafkaOptionsName.OtherConsumer, stoppingToken);

        await Task.WhenAll(t1, t2);
    }

    private async Task RunPipelineAsync(string consumerKey, CancellationToken cancellationToken)
    {
        using IServiceScope scope = _scopeFactory.CreateScope();

        IKafkaConsumer<OrderProcessingKey, OrderProcessingValue> consumer = scope.ServiceProvider
            .GetRequiredKeyedService<IKafkaConsumer<OrderProcessingKey, OrderProcessingValue>>(consumerKey);

        int batchSize = _backgroundOptions.CurrentValue.MaxBatchSize;
        var maxWait = TimeSpan.FromMilliseconds(_backgroundOptions.CurrentValue.MaxBatchWaitMilliseconds);

        var channel = Channel.CreateBounded<ConsumeResult<OrderProcessingKey, OrderProcessingValue>>(
            new BoundedChannelOptions(batchSize * _kafkaChannelOptions.Value.CapacityMultiplier)
            {
                SingleWriter = _kafkaChannelOptions.Value.SingleWriter,
                SingleReader = _kafkaChannelOptions.Value.SingleReader,
                FullMode = _kafkaChannelOptions.Value.FullMode,
            });

        var reader = new OrderProcessingChannelReader(consumer);
        var handler = new OrderProcessingChannelBatchHandler(_scopeFactory, consumer, batchSize, maxWait);

        await Task.WhenAll(
            reader.ReadAsync(channel.Writer, cancellationToken),
            handler.HandleAsync(channel.Reader, cancellationToken));
    }
}
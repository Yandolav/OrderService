using Confluent.Kafka;
using Kafka.Consumer.Handlers;
using Kafka.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Orders.Kafka.Contracts;

namespace Kafka.Consumer.BackgroundServices;

public class OrderProcessingBackgroundService : BackgroundService
{
    private readonly IOptionsMonitor<BackgroundServiceOptions> _options;
    private readonly IServiceScopeFactory _scopeFactory;

    public OrderProcessingBackgroundService(
        IOptionsMonitor<BackgroundServiceOptions> options,
        IServiceScopeFactory scopeFactory)
    {
        _options = options;
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using IServiceScope scope = _scopeFactory.CreateScope();
        IMessageHandler handler = scope.ServiceProvider.GetRequiredService<IMessageHandler>();
        IKafkaConsumer<OrderProcessingKey, OrderProcessingValue> consumer = scope.ServiceProvider.GetRequiredService<IKafkaConsumer<OrderProcessingKey, OrderProcessingValue>>();
        var batch = new List<ConsumeResult<OrderProcessingKey, OrderProcessingValue>>(_options.CurrentValue.MaxBatchSize);

        await Task.Yield();

        while (!stoppingToken.IsCancellationRequested)
        {
            ConsumeResult<OrderProcessingKey, OrderProcessingValue> message = consumer.Consumer(stoppingToken);
            batch.Add(message);

            if (batch.Count >= _options.CurrentValue.MaxBatchSize)
            {
                await handler.HandleASync(batch, stoppingToken);
                consumer.Commit();
                batch.Clear();
            }
        }
    }
}
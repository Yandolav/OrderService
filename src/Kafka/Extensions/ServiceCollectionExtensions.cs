using Confluent.Kafka;
using Core.Application.Ports.SecondaryPorts;
using Kafka.Consumer;
using Kafka.Consumer.Handlers;
using Kafka.Options;
using Kafka.Producer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Orders.Kafka.Contracts;

namespace Kafka.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddKafka(this IServiceCollection services)
    {
        services.AddSingleton(_ => OrderProcessingKey.Parser);
        services.AddSingleton(_ => OrderProcessingValue.Parser);
        services.AddSingleton(_ => OrderCreationKey.Parser);
        services.AddSingleton(_ => OrderCreationValue.Parser);

        services.AddSingleton<ISerializer<OrderCreationKey>, ProtobufSerializer<OrderCreationKey>>();
        services.AddSingleton<ISerializer<OrderCreationValue>, ProtobufSerializer<OrderCreationValue>>();
        services.AddSingleton<IDeserializer<OrderProcessingKey>, ProtobufDeserializer<OrderProcessingKey>>();
        services.AddSingleton<IDeserializer<OrderProcessingValue>, ProtobufDeserializer<OrderProcessingValue>>();

        RegisterProducer(services, KafkaOptionsName.MainProducer);
        RegisterProducer(services, KafkaOptionsName.OtherProducer);
        services.AddSingleton<IOrderEventsProducer, OrderEventsProducer>();

        RegisterConsumer(services, KafkaOptionsName.MainConsumer);
        RegisterConsumer(services, KafkaOptionsName.OtherConsumer);
        services.AddScoped<IMessageHandler, MessageHandler>();

        return services;
    }

    private static void RegisterProducer(IServiceCollection services, string producerName)
    {
        services.AddKeyedSingleton<IKafkaProducer<OrderCreationKey, OrderCreationValue>>(
            producerName,
            (sp, _) =>
            {
                KafkaProducerOptions opts = sp.GetRequiredService<IOptionsMonitor<KafkaProducerOptions>>().Get(producerName);

                return new KafkaProducer<OrderCreationKey, OrderCreationValue>(
                    new OptionsWrapper<KafkaProducerOptions>(opts),
                    sp.GetRequiredService<ISerializer<OrderCreationKey>>(),
                    sp.GetRequiredService<ISerializer<OrderCreationValue>>(),
                    sp.GetRequiredService<ILogger<KafkaProducer<OrderCreationKey, OrderCreationValue>>>());
            });
    }

    private static void RegisterConsumer(IServiceCollection services, string consumerName)
    {
        services.AddKeyedScoped<IKafkaConsumer<OrderProcessingKey, OrderProcessingValue>>(
            consumerName,
            (sp, _) =>
            {
                KafkaConsumerOptions opts = sp.GetRequiredService<IOptionsMonitor<KafkaConsumerOptions>>().Get(consumerName);

                return new KafkaConsumer<OrderProcessingKey, OrderProcessingValue>(
                    sp.GetRequiredService<IOptions<KafkaTopicsOptions>>(),
                    new OptionsWrapper<KafkaConsumerOptions>(opts),
                    sp.GetRequiredService<IDeserializer<OrderProcessingKey>>(),
                    sp.GetRequiredService<IDeserializer<OrderProcessingValue>>());
            });
    }
}
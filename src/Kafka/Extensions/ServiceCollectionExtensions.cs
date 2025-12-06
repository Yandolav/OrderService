using Confluent.Kafka;
using Core.Application.Ports.SecondaryPorts;
using Kafka.Consumer;
using Kafka.Consumer.Handlers;
using Kafka.Producer;
using Microsoft.Extensions.DependencyInjection;
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

        services.AddSingleton<IKafkaProducer<OrderCreationKey, OrderCreationValue>, KafkaProducer<OrderCreationKey, OrderCreationValue>>();
        services.AddSingleton<IOrderEventsProducer, OrderEventsProducer>();

        services.AddScoped<IKafkaConsumer<OrderProcessingKey, OrderProcessingValue>, KafkaConsumer<OrderProcessingKey, OrderProcessingValue>>();
        services.AddScoped<IMessageHandler, MessageHandler>();

        return services;
    }
}
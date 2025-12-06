using Confluent.Kafka;
using Google.Protobuf;

namespace Kafka.Producer;

public class ProtobufSerializer<T> : ISerializer<T> where T : class, IMessage
{
    public byte[] Serialize(T data, SerializationContext context)
    {
        return data is null ? [] : data.ToByteArray();
    }
}
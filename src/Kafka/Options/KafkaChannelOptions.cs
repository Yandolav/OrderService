using System.Threading.Channels;

namespace Kafka.Options;

public class KafkaChannelOptions
{
    public int CapacityMultiplier { get; set; }

    public bool SingleWriter { get; set; }

    public bool SingleReader { get; set; }

    public BoundedChannelFullMode FullMode { get; set; }
}
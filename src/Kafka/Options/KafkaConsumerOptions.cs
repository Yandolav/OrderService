namespace Kafka.Options;

public class KafkaConsumerOptions
{
    public string? BootstrapServers { get; set; }

    public string? GroupId { get; set; }

    public bool EnableAutoCommit { get; set; }

    public string? GroupInstanceId { get; set; }
}
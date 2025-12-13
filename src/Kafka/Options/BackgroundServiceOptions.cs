namespace Kafka.Options;

public class BackgroundServiceOptions
{
    public int MaxBatchSize { get; set; }

    public int MaxBatchWaitMilliseconds { get; set; }
}
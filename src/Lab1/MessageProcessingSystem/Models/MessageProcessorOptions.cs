using System.Threading.Channels;

namespace Lab1.MessageProcessingSystem.Models;

public sealed class MessageProcessorOptions
{
    public int ChannelCapacity { get; init; } = 10000;

    public bool SingleReader { get; init; } = true;

    public bool SingleWriter { get; init; } = false;

    public BoundedChannelFullMode FullMode { get; init; } = BoundedChannelFullMode.DropOldest;

    public int BatchSize { get; init; } = 16;

    public TimeSpan BatchTimeout { get; init; } = TimeSpan.FromMilliseconds(250);

    public TimeSpan BatchTimeoutCheckPeriod { get; init; } = TimeSpan.FromMilliseconds(50);
}
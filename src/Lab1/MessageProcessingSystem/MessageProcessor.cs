using Lab1.MessageProcessingSystem.Interfaces;
using Lab1.MessageProcessingSystem.Models;
using System.Threading.Channels;

namespace Lab1.MessageProcessingSystem;

public sealed class MessageProcessor : IMessageSender, IMessageProcessor
{
    private readonly Channel<Message> _channel;
    private readonly IReadOnlyList<IMessageHandler> _handlers;
    private readonly MessageProcessorOptions _options;

    public MessageProcessor(IEnumerable<IMessageHandler> handlers, MessageProcessorOptions? options = null)
    {
        _handlers = handlers is IReadOnlyList<IMessageHandler> list ? list : new List<IMessageHandler>(handlers);
        _options = options ?? new MessageProcessorOptions();

        var channelOptions = new BoundedChannelOptions(_options.ChannelCapacity)
        {
            SingleReader = _options.SingleReader,
            SingleWriter = _options.SingleWriter,
            FullMode = _options.FullMode,
        };

        _channel = Channel.CreateBounded<Message>(channelOptions);
    }

    public ValueTask SendAsync(Message message, CancellationToken cancellationToken) =>
        _channel.Writer.WriteAsync(message, cancellationToken);

    public async Task ProcessAsync(CancellationToken cancellationToken)
    {
        IAsyncEnumerable<Message> stream = _channel.Reader.ReadAllAsync(cancellationToken);

        await foreach (IReadOnlyList<Message> batch in stream.ChunkAsync(
                           _options.BatchSize,
                           _options.BatchTimeout,
                           _options.BatchTimeoutCheckPeriod))
        {
            foreach (IMessageHandler handler in _handlers)
            {
                await handler.HandleAsync(batch, cancellationToken);
            }
        }
    }

    public void Complete() => _channel.Writer.TryComplete();
}
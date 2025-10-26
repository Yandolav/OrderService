using Lab1.MessageProcessingSystem.Models;

namespace Lab1.MessageProcessingSystem.Interfaces;

public interface IMessageHandler
{
    ValueTask HandleAsync(IEnumerable<Message> messages, CancellationToken cancellationToken);
}
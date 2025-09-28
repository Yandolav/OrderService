using Lab1.MessageProcessingSystem.Models;

namespace Lab1.MessageProcessingSystem.Interfaces;

public interface IMessageSender
{
    ValueTask SendAsync(Message message, CancellationToken cancellationToken);
}
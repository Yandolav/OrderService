namespace Lab1.MessageProcessingSystem.Interfaces;

public interface IMessageProcessor
{
    Task ProcessAsync(CancellationToken cancellationToken);

    void Complete();
}
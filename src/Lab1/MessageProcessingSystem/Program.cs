using Lab1.MessageProcessingSystem;
using Lab1.MessageProcessingSystem.Models;

var implementation = new MessageProcessor([new ConsoleMessageHandler()]);
MessageProcessor processor = implementation;
MessageProcessor sender = implementation;

Task processingTask = processor.ProcessAsync(CancellationToken.None);

await Parallel.ForEachAsync(
    Enumerable.Range(1, 100),
    new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount },
    async (i, ct) =>
    {
        await sender.SendAsync(new Message($"Title {i}", $"Text {i}"), ct);
    });

processor.Complete();
await processingTask;
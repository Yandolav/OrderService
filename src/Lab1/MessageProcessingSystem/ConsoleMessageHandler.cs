using Lab1.MessageProcessingSystem.Interfaces;
using Lab1.MessageProcessingSystem.Models;
using System.Text;

namespace Lab1.MessageProcessingSystem;

public sealed class ConsoleMessageHandler : IMessageHandler
{
    public ValueTask HandleAsync(IEnumerable<Message> messages, CancellationToken cancellationToken)
    {
        var sb = new StringBuilder();
        foreach (Message m in messages)
        {
            sb.Append(m.Title).Append(": ").Append(m.Text).AppendLine();
        }

        Console.WriteLine(sb.ToString());
        return default;
    }
}
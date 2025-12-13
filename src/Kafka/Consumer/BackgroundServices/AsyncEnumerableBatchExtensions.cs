using System.Runtime.CompilerServices;

namespace Kafka.Consumer.BackgroundServices;

public static class AsyncEnumerableBatchExtensions
{
    public static async IAsyncEnumerable<IReadOnlyList<T>> ChunkBySizeOrInactivityTimeout<T>(
        this IAsyncEnumerable<T> source,
        int maxSize,
        TimeSpan timeout,
        TimeSpan checkInterval,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var buffer = new List<T>(maxSize);

        await using IAsyncEnumerator<T> enumerator = source.GetAsyncEnumerator(cancellationToken);

        while (!cancellationToken.IsCancellationRequested)
        {
            if (buffer.Count == 0)
            {
                if (!await enumerator.MoveNextAsync())
                {
                    yield break;
                }

                buffer.Add(enumerator.Current);

                if (buffer.Count >= maxSize)
                {
                    yield return buffer.ToArray();
                    buffer.Clear();
                }

                continue;
            }

            ValueTask<bool> moveNext = enumerator.MoveNextAsync();
            TimeSpan remaining = timeout;

            while (!moveNext.IsCompleted && remaining > TimeSpan.Zero && !cancellationToken.IsCancellationRequested)
            {
                TimeSpan delay = remaining <= checkInterval ? remaining : checkInterval;
                await Task.Delay(delay, cancellationToken);
                remaining -= delay;
            }

            if (!moveNext.IsCompleted && buffer.Count > 0)
            {
                yield return buffer.ToArray();
                buffer.Clear();
            }

            bool hasNext = await moveNext;

            if (!hasNext)
            {
                if (buffer.Count > 0)
                    yield return buffer.ToArray();

                yield break;
            }

            buffer.Add(enumerator.Current);

            if (buffer.Count >= maxSize)
            {
                yield return buffer.ToArray();
                buffer.Clear();
            }
        }
    }
}
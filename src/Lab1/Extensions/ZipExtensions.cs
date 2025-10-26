namespace Lab1.Extensions;

public static class ZipExtensions
{
    public static IEnumerable<T[]> ZipExtension<T>(this IEnumerable<T> source, params IEnumerable<T>[] others)
    {
        ArgumentNullException.ThrowIfNull(source);
        foreach (IEnumerable<T> item in others)
        {
            ArgumentNullException.ThrowIfNull(item);
        }

        var enumerators = new IEnumerator<T>[others.Length + 1];
        try
        {
            enumerators[0] = source.GetEnumerator();
            for (int i = 0; i < others.Length; i++)
            {
                enumerators[i + 1] = others[i].GetEnumerator();
            }

            while (true)
            {
                for (int i = 0; i < enumerators.Length; i++)
                {
                    if (!enumerators[i].MoveNext())
                        yield break;
                }

                var row = new T[enumerators.Length];
                for (int i = 0; i < enumerators.Length; i++)
                    row[i] = enumerators[i].Current;

                yield return row;
            }
        }
        finally
        {
            foreach (IEnumerator<T> e in enumerators)
                e.Dispose();
        }
    }

    public static async IAsyncEnumerable<T[]> ZipAsyncExtension<T>(this IAsyncEnumerable<T>? source, params IAsyncEnumerable<T>[] others)
    {
        ArgumentNullException.ThrowIfNull(source);
        foreach (IAsyncEnumerable<T> item in others)
        {
            ArgumentNullException.ThrowIfNull(item);
        }

        var enumerators = new IAsyncEnumerator<T>[others.Length + 1];
        try
        {
            enumerators[0] = source.GetAsyncEnumerator();
            for (int i = 0; i < others.Length; i++)
            {
                enumerators[i + 1] = others[i].GetAsyncEnumerator();
            }

            while (true)
            {
                for (int i = 0; i < enumerators.Length; i++)
                {
                    if (!await enumerators[i].MoveNextAsync().ConfigureAwait(false))
                        yield break;
                }

                var row = new T[enumerators.Length];
                for (int i = 0; i < enumerators.Length; i++)
                    row[i] = enumerators[i].Current;

                yield return row;
            }
        }
        finally
        {
            foreach (IAsyncEnumerator<T> e in enumerators)
                await e.DisposeAsync().ConfigureAwait(false);
        }
    }
}
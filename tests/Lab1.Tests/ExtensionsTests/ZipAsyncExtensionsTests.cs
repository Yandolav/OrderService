using FluentAssertions;
using Lab1.Extensions;
using Xunit;

namespace Lab1.Tests.ExtensionsTests;

public class ZipAsyncExtensionsTests
{
    [Fact]
    public async Task ZipAsync_NoOthers_ReturnsSingleElementRows()
    {
        // arrange
        int[] source = new[] { 1, 2, 3 };
        int[][] expected = source.Select(x => new[] { x }).ToArray();

        // act
        int[][] result = await CollectAsync(ToAsync(source).ZipAsyncExtension());

        // assert
        result.Should().BeEquivalentTo(expected, o => o.WithStrictOrdering());
    }

    [Theory]
    [MemberData(nameof(ZipData.EqualLengthCases), MemberType = typeof(ZipData))]
    public async Task ZipAsync_WithEqualLengths_ReturnsAllRows(int[] source, int[][] others)
    {
        // arrange
        IAsyncEnumerable<int> src = ToAsync(source);
        IAsyncEnumerable<int>[] oth = others.Select(ToAsync).ToArray();

        int[][] expected = Enumerable.Range(0, source.Length)
            .Select(i =>
            {
                int[] row = new int[1 + others.Length];
                row[0] = source[i];
                for (int j = 0; j < others.Length; j++)
                {
                    row[j + 1] = others[j][i];
                }

                return row;
            })
            .ToArray();

        // act
        int[][] result = await CollectAsync(src.ZipAsyncExtension(oth));

        // assert
        result.Should().BeEquivalentTo(expected, o => o.WithStrictOrdering());
    }

    [Theory]
    [MemberData(nameof(ZipData.DifferentLengthCases), MemberType = typeof(ZipData))]
    public async Task ZipAsync_WithDifferentLengths_TruncatesToShortest(int[] source, int[][] others)
    {
        // arrange
        IAsyncEnumerable<int> src = ToAsync(source);
        IAsyncEnumerable<int>[] oth = others.Select(ToAsync).ToArray();
        int minLen = new[] { source.Length }.Concat(others.Select(o => o.Length)).Min();

        int[][] expected = Enumerable.Range(0, minLen)
            .Select(i =>
            {
                int[] row = new int[1 + others.Length];
                row[0] = source[i];
                for (int j = 0; j < others.Length; j++)
                {
                    row[j + 1] = others[j][i];
                }

                return row;
            })
            .ToArray();

        // act
        int[][] result = await CollectAsync(src.ZipAsyncExtension(oth));

        // assert
        result.Should().BeEquivalentTo(expected, o => o.WithStrictOrdering());
    }

    private static async IAsyncEnumerable<T> ToAsync<T>(IEnumerable<T> source)
    {
        foreach (T item in source)
        {
            await Task.Yield();
            yield return item;
        }
    }

    private static async Task<T[]> CollectAsync<T>(IAsyncEnumerable<T> source)
    {
        var list = new List<T>();
        await foreach (T item in source)
        {
            list.Add(item);
        }

        return list.ToArray();
    }
}
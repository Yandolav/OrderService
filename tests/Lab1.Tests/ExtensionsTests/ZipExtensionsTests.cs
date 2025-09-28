using FluentAssertions;
using Lab1.Extensions;
using Xunit;

namespace Lab1.Tests.ExtensionsTests;

public class ZipExtensionsTests
{
    [Fact]
    public void Zip_NoOthers_ReturnsSingleElementRows()
    {
        // arrange
        int[] source = new[] { 1, 2, 3 };
        int[][] expected = source.Select(x => new[] { x }).ToArray();

        // act
        int[][] result = source.ZipExtension().ToArray();

        // assert
        result.Should().BeEquivalentTo(expected, o => o.WithStrictOrdering());
    }

    [Theory]
    [MemberData(nameof(ZipData.EqualLengthCases), MemberType = typeof(ZipData))]
    public void Zip_WithEqualLengths_ReturnsAllRows(int[] source, int[][] others)
    {
        // arrange
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
        int[][] result = source.ZipExtension(others).ToArray();

        // assert
        result.Should().BeEquivalentTo(expected, o => o.WithStrictOrdering());
    }

    [Theory]
    [MemberData(nameof(ZipData.DifferentLengthCases), MemberType = typeof(ZipData))]
    public void Zip_WithDifferentLengths_TruncatesToShortest(int[] source, int[][] others)
    {
        // arrange
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
        int[][] result = source.ZipExtension(others).ToArray();

        // assert
        result.Should().BeEquivalentTo(expected, o => o.WithStrictOrdering());
    }
}
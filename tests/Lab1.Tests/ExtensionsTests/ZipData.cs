namespace Lab1.Tests.ExtensionsTests;

public static class ZipData
{
    public static IEnumerable<object[]> EqualLengthCases()
    {
        yield return new object[]
        {
            new[] { 1, 2, 3 },
            new[] { new[] { 10, 20, 30 } },
        };

        yield return new object[]
        {
            new[] { 1, 2, 3 },
            new[] { new[] { 10, 20, 30 }, new[] { 100, 200, 300 } },
        };
    }

    public static IEnumerable<object[]> DifferentLengthCases()
    {
        yield return new object[]
        {
            new[] { 1, 2, 3, 4 },
            new[] { new[] { 10, 20 }, new[] { 100, 200, 300, 400 } },
        };

        yield return new object[]
        {
            new[] { 1, 2 },
            new[] { new[] { 10, 20, 30 }, new[] { 100, 200, 300, 400 } },
        };

        yield return new object[]
        {
            new[] { 1, 2, 3, 4 },
            new[] { new[] { 10 }, new[] { 100, 200, 300 } },
        };
    }
}
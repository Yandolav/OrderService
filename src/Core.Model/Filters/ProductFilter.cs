namespace Core.Model.Filters;

public sealed record ProductFilter(long[] Ids, decimal? MinPrice = null, decimal? MaxPrice = null, string? NameContains = null);
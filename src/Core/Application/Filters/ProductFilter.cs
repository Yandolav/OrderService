namespace Domain.Entities.Filters;

public sealed record ProductFilter(long[]? Ids = null, decimal? MinPrice = null, decimal? MaxPrice = null, string? NameContains = null);
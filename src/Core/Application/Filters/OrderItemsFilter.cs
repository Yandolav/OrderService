namespace Domain.Entities.Filters;

public sealed record OrderItemsFilter(long[]? OrderIds = null, long[]? ProductIds = null, bool? Deleted = null);
namespace Core.Model.Filters;

public sealed record OrderItemsFilter(long[] OrderIds, long[] ProductIds, bool? Deleted = null);
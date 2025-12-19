using Core.Model.Enums;

namespace Core.Model.Filters;

public sealed record OrderFilter(long[] Ids, OrderState? State = null, string? CreatedBy = null);
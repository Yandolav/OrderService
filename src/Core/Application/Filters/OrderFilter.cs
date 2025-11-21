using Core.Domain.Enums;

namespace Core.Application.Filters;

public sealed record OrderFilter(long[] Ids, OrderState? State = null, string? CreatedBy = null);
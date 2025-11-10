using Domain.Enums;

namespace Domain.Entities.Filters;

public sealed record OrderFilter(long[]? Ids = null, OrderState? State = null, string? CreatedBy = null);
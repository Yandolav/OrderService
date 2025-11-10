using Core.Domain.Enums;

namespace Core.Application.Filters;

public sealed record OrderFilter(long[]? Ids = null, OrderState? State = null, string? CreatedBy = null);
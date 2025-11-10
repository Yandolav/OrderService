using Domain.Enums;

namespace Domain.Entities.Filters;

public sealed record OrderHistoryFilter(long[]? OrderIds = null, OrderHistoryItemKind? Kind = null);
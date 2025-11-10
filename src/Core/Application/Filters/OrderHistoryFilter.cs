using Core.Domain.Enums;

namespace Core.Application.Filters;

public sealed record OrderHistoryFilter(long[]? OrderIds = null, OrderHistoryItemKind? Kind = null);
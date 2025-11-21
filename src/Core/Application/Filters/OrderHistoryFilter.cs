using Core.Domain.Enums;

namespace Core.Application.Filters;

public sealed record OrderHistoryFilter(long[] OrderIds, OrderHistoryItemKind? Kind = null);
using Core.Model.Enums;

namespace Core.Model.Filters;

public sealed record OrderHistoryFilter(long[] OrderIds, OrderHistoryItemKind? Kind = null);
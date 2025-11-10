using Core.Domain.Enums;
using Core.Domain.Payloads;

namespace Core.Domain.Entities;

public sealed record OrderHistory(long OrderHistoryItemId, long OrderId, DateTimeOffset OrderHistoryItemCreatedAt, OrderHistoryItemKind OrderHistoryItemKind, IOrderHistoryPayload OrderHistoryItemPayload);
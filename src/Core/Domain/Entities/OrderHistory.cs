using Domain.Entities.Payloads;
using Domain.Enums;

namespace Domain.Entities;

public sealed record OrderHistory(long OrderHistoryItemId, long OrderId, DateTimeOffset OrderHistoryItemCreatedAt, OrderHistoryItemKind OrderHistoryItemKind, IOrderHistoryPayload OrderHistoryItemPayload);
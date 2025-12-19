using Core.Model.Enums;
using Core.Model.Payloads;

namespace Core.Model.Entities;

public sealed record OrderHistory(long OrderHistoryItemId, long OrderId, DateTimeOffset OrderHistoryItemCreatedAt, OrderHistoryItemKind OrderHistoryItemKind, IOrderHistoryPayload OrderHistoryItemPayload);
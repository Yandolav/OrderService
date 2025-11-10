using HttpGateway.Models.Payloads;

namespace HttpGateway.Models.OrderHistory;

public sealed class OrderHistoryItemDto
{
    public OrderHistoryItemDto(
        long orderHistoryItemId,
        long orderId,
        DateTimeOffset createdAt,
        OrderHistoryItemKindDto kind,
        OrderHistoryItemPayloadDto payload)
    {
        OrderHistoryItemId = orderHistoryItemId;
        OrderId = orderId;
        CreatedAt = createdAt;
        Kind = kind;
        Payload = payload;
    }

    public long OrderHistoryItemId { get; }

    public long OrderId { get; }

    public DateTimeOffset CreatedAt { get; }

    public OrderHistoryItemKindDto Kind { get; }

    public OrderHistoryItemPayloadDto Payload { get; }
}
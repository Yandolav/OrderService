namespace HttpGateway.Models.AddOrderItem;

public sealed class AddOrderItemResponseDto
{
    public AddOrderItemResponseDto(long orderItemId)
    {
        OrderItemId = orderItemId;
    }

    public long OrderItemId { get; }
}
namespace Task3.HttpGateway.Models;

public sealed class AddOrderItemResponseDto
{
    public AddOrderItemResponseDto(long orderItemId)
    {
        OrderItemId = orderItemId;
    }

    public long OrderItemId { get; }
}
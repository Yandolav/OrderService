namespace HttpGateway.Models.CreateOrder;

public sealed class CreateOrderResponseDto
{
    public CreateOrderResponseDto(long orderId)
    {
        OrderId = orderId;
    }

    public long OrderId { get; }
}
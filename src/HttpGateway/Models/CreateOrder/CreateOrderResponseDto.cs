namespace Task3.HttpGateway.Models;

public sealed class CreateOrderResponseDto
{
    public CreateOrderResponseDto(long orderId)
    {
        OrderId = orderId;
    }

    public long OrderId { get; }
}
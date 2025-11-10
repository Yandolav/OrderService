namespace Task3.HttpGateway.Models;

public sealed class ItemAddedPayloadDto : OrderHistoryItemPayloadDto
{
    public ItemAddedPayloadDto(long productId, int quantity)
    {
        ProductId = productId;
        Quantity = quantity;
    }

    public long ProductId { get; }

    public int Quantity { get; }
}
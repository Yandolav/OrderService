namespace Task3.HttpGateway.Models;

public sealed class ItemRemovedPayloadDto : OrderHistoryItemPayloadDto
{
    public ItemRemovedPayloadDto(long productId, int quantity)
    {
        ProductId = productId;
        Quantity = quantity;
    }

    public long ProductId { get; }

    public int Quantity { get; }
}
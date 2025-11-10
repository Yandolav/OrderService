namespace Task3.HttpGateway.Models;

public sealed class OrderCreatedPayloadDto : OrderHistoryItemPayloadDto
{
    public OrderCreatedPayloadDto(string createdBy)
    {
        CreatedBy = createdBy;
    }

    public string CreatedBy { get; }
}
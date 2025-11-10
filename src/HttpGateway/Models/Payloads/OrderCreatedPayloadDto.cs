namespace HttpGateway.Models.Payloads;

public sealed class OrderCreatedPayloadDto : OrderHistoryItemPayloadDto
{
    public OrderCreatedPayloadDto(string createdBy)
    {
        CreatedBy = createdBy;
    }

    public string CreatedBy { get; }
}
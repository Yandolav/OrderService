namespace HttpGateway.Models.Payloads;

public class DeliveryStartedPayloadDto : OrderHistoryItemPayloadDto
{
    public DeliveryStartedPayloadDto(string deliveredBy, DateTimeOffset startedAt)
    {
        DeliveredBy = deliveredBy;
        StartedAt = startedAt;
    }

    public string DeliveredBy { get; }

    public DateTimeOffset StartedAt { get; }
}
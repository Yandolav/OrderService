namespace HttpGateway.Models.Payloads;

public class PackingStartedPayloadDto : OrderHistoryItemPayloadDto
{
    public PackingStartedPayloadDto(string packingBy, DateTimeOffset startedAt)
    {
        PackingBy = packingBy;
        StartedAt = startedAt;
    }

    public string PackingBy { get; }

    public DateTimeOffset StartedAt { get; }
}
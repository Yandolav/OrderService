namespace HttpGateway.Models.Payloads;

public class PackingFinishedPayloadDto : OrderHistoryItemPayloadDto
{
    public PackingFinishedPayloadDto(DateTimeOffset finishedAt, bool isSuccessful, string? failureReason)
    {
        FinishedAt = finishedAt;
        IsSuccessful = isSuccessful;
        FailureReason = failureReason;
    }

    public DateTimeOffset FinishedAt { get; }

    public bool IsSuccessful { get; }

    public string? FailureReason { get; }
}
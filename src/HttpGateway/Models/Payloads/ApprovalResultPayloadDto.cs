namespace HttpGateway.Models.Payloads;

public class ApprovalResultPayloadDto : OrderHistoryItemPayloadDto
{
    public ApprovalResultPayloadDto(bool isApproved, string createdBy, DateTimeOffset createdAt)
    {
        IsApproved = isApproved;
        CreatedBy = createdBy;
        CreatedAt = createdAt;
    }

    public bool IsApproved { get; }

    public string CreatedBy { get; }

    public DateTimeOffset CreatedAt { get; }
}
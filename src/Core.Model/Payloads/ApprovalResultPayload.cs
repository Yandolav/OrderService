namespace Core.Model.Payloads;

public sealed record ApprovalResultPayload(bool IsApproved, string CreatedBy, DateTimeOffset CreatedAt) : IOrderHistoryPayload;
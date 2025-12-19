namespace Core.Model.Payloads;

public sealed record OrderCreatedPayload(string CreatedBy) : IOrderHistoryPayload;
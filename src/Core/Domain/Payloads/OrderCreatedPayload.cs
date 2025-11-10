namespace Core.Domain.Payloads;

public sealed record OrderCreatedPayload(string CreatedBy) : IOrderHistoryPayload;
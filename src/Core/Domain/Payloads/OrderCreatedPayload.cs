namespace Domain.Entities.Payloads;

public sealed record OrderCreatedPayload(string CreatedBy) : IOrderHistoryPayload;
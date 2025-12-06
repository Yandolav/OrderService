namespace Core.Domain.Payloads;

public sealed record PackingStartedPayload(string PackingBy, DateTimeOffset StartedAt) : IOrderHistoryPayload;
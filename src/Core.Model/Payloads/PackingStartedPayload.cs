namespace Core.Model.Payloads;

public sealed record PackingStartedPayload(string PackingBy, DateTimeOffset StartedAt) : IOrderHistoryPayload;
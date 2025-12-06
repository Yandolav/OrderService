namespace Core.Domain.Payloads;

public sealed record DeliveryStartedPayload(string DeliveredBy, DateTimeOffset StartedAt) : IOrderHistoryPayload;
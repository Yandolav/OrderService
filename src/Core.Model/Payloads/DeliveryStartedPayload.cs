namespace Core.Model.Payloads;

public sealed record DeliveryStartedPayload(string DeliveredBy, DateTimeOffset StartedAt) : IOrderHistoryPayload;
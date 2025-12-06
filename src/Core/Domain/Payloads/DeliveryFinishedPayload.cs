namespace Core.Domain.Payloads;

public sealed record DeliveryFinishedPayload(DateTimeOffset FinishedAt, bool IsSuccessful, string? FailureReason) : IOrderHistoryPayload;
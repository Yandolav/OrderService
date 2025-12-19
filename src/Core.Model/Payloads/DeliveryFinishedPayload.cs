namespace Core.Model.Payloads;

public sealed record DeliveryFinishedPayload(DateTimeOffset FinishedAt, bool IsSuccessful, string? FailureReason) : IOrderHistoryPayload;
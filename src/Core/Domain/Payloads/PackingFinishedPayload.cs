namespace Core.Domain.Payloads;

public sealed record PackingFinishedPayload(DateTimeOffset FinishedAt, bool IsSuccessful, string? FailureReason) : IOrderHistoryPayload;
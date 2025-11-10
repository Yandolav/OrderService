namespace Core.Domain.Payloads;

public sealed record ItemAddedPayload(long ProductId, int Quantity) : IOrderHistoryPayload;
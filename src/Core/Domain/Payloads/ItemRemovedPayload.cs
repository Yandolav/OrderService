namespace Core.Domain.Payloads;

public sealed record ItemRemovedPayload(long ProductId, int Quantity) : IOrderHistoryPayload;
namespace Domain.Entities.Payloads;

public sealed record ItemRemovedPayload(long ProductId, int Quantity) : IOrderHistoryPayload;
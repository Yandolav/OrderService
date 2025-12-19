namespace Core.Model.Payloads;

public sealed record ItemRemovedPayload(long ProductId, int Quantity) : IOrderHistoryPayload;
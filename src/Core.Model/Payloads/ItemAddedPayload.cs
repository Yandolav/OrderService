namespace Core.Model.Payloads;

public sealed record ItemAddedPayload(long ProductId, int Quantity) : IOrderHistoryPayload;
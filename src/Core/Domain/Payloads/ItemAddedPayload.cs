namespace Domain.Entities.Payloads;

public sealed record ItemAddedPayload(long ProductId, int Quantity) : IOrderHistoryPayload;
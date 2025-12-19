namespace Core.Model.Entities;

public sealed record OrderItem(long OrderItemId, long OrderId, long ProductId, int OrderItemQuantity, bool OrderItemDeleted);
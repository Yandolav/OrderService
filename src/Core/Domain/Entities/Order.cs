using Domain.Enums;

namespace Domain.Entities;

public sealed record Order(long OrderId, OrderState OrderState, DateTimeOffset OrderCreatedAt, string OrderCreatedBy);
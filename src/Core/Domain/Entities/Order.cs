using Core.Domain.Enums;

namespace Core.Domain.Entities;

public sealed record Order(long OrderId, OrderState OrderState, DateTimeOffset OrderCreatedAt, string OrderCreatedBy);
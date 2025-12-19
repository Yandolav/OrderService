using Core.Model.Enums;

namespace Core.Model.Entities;

public sealed record Order(long OrderId, OrderState OrderState, DateTimeOffset OrderCreatedAt, string OrderCreatedBy);
using Domain.Enums;

namespace Domain.Entities.Payloads;

public sealed record StateChangedPayload(OrderState OldState, OrderState NewState) : IOrderHistoryPayload;
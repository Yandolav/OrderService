using Core.Domain.Enums;

namespace Core.Domain.Payloads;

public sealed record StateChangedPayload(OrderState OldState, OrderState NewState) : IOrderHistoryPayload;
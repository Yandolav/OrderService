using Core.Model.Enums;

namespace Core.Model.Payloads;

public sealed record StateChangedPayload(OrderState OldState, OrderState NewState) : IOrderHistoryPayload;
namespace HttpGateway.Models.Payloads;

public sealed class StateChangedPayloadDto : OrderHistoryItemPayloadDto
{
    public StateChangedPayloadDto(OrderStateDto oldState, OrderStateDto newState)
    {
        OldState = oldState;
        NewState = newState;
    }

    public OrderStateDto OldState { get; }

    public OrderStateDto NewState { get; }
}
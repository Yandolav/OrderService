namespace HttpGateway.Models.OrderHistory;

public enum OrderHistoryItemKindDto
{
    Unspecified = 0,
    CreatedItem = 1,
    ItemAdded = 2,
    ItemRemoved = 3,
    StateChanged = 4,
    ApprovalReceived = 5,
    PackingStarted = 6,
    PackingFinished = 7,
    DeliveryStarted = 8,
    DeliveryFinished = 9,
}
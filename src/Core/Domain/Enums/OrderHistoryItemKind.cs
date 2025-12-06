namespace Core.Domain.Enums;

public enum OrderHistoryItemKind
{
    Created = 0,
    ItemAdded = 1,
    ItemRemoved = 2,
    StateChanged = 3,
    ApprovalReceived = 4,
    PackingStarted = 5,
    PackingFinished = 6,
    DeliveryStarted = 7,
    DeliveryFinished = 8,
}
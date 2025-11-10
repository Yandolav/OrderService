namespace HttpGateway.Models.OrderHistory;

public enum OrderHistoryItemKindDto
{
    Unspecified = 0,
    CreatedItem = 1,
    ItemAdded = 2,
    ItemRemoved = 3,
    StateChanged = 4,
}
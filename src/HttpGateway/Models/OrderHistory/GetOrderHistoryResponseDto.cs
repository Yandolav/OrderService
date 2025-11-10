namespace HttpGateway.Models.OrderHistory;

public sealed class GetOrderHistoryResponseDto
{
    public GetOrderHistoryResponseDto(IReadOnlyList<OrderHistoryItemDto> items)
    {
        Items = items;
    }

    public IReadOnlyList<OrderHistoryItemDto> Items { get; }
}
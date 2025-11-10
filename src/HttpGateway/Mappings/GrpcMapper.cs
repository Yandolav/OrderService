using HttpGateway.Models;
using HttpGateway.Models.OrderHistory;
using HttpGateway.Models.Payloads;
using Presentation.Grpc;

namespace HttpGateway.Mappings;

public sealed class GrpcMapper
{
    public OrderStateDto MapOrderState(OrderState state)
    {
        return state switch
        {
            OrderState.Unspecified => OrderStateDto.Unspecified,
            OrderState.Created => OrderStateDto.Created,
            OrderState.Processing => OrderStateDto.Processing,
            OrderState.Completed => OrderStateDto.Completed,
            OrderState.Cancelled => OrderStateDto.Cancelled,
            _ => throw new ArgumentOutOfRangeException(nameof(state)),
        };
    }

    public OrderHistoryItemKindDto MapHistoryKind(OrderHistoryItemKind kind)
    {
        return kind switch
        {
            OrderHistoryItemKind.Unspecified => OrderHistoryItemKindDto.Unspecified,
            OrderHistoryItemKind.CreatedItem => OrderHistoryItemKindDto.CreatedItem,
            OrderHistoryItemKind.ItemAdded => OrderHistoryItemKindDto.ItemAdded,
            OrderHistoryItemKind.ItemRemoved => OrderHistoryItemKindDto.ItemRemoved,
            OrderHistoryItemKind.StateChanged => OrderHistoryItemKindDto.StateChanged,
            _ => throw new ArgumentOutOfRangeException(nameof(kind)),
        };
    }

    public OrderHistoryItemDto MapHistoryItem(OrderHistoryItem item)
    {
        ArgumentNullException.ThrowIfNull(item);

        OrderHistoryItemPayloadDto payload;
        switch (item.Kind)
        {
            case OrderHistoryItemKind.CreatedItem:
            {
                if (item.OrderCreated is null) throw new ArgumentOutOfRangeException(nameof(item), "History payload 'order_created' is missing.");
                payload = new OrderCreatedPayloadDto(item.OrderCreated.CreatedBy);
                break;
            }

            case OrderHistoryItemKind.ItemAdded:
            {
                if (item.ItemAdded is null) throw new ArgumentOutOfRangeException(nameof(item), "History payload 'item_added' is missing.");
                payload = new ItemAddedPayloadDto(item.ItemAdded.ProductId, item.ItemAdded.Quantity);
                break;
            }

            case OrderHistoryItemKind.ItemRemoved:
            {
                if (item.ItemRemoved is null) throw new ArgumentOutOfRangeException(nameof(item), "History payload 'item_removed' is missing.");
                payload = new ItemRemovedPayloadDto(item.ItemRemoved.ProductId, item.ItemRemoved.Quantity);
                break;
            }

            case OrderHistoryItemKind.StateChanged:
            {
                if (item.StateChanged is null) throw new ArgumentOutOfRangeException(nameof(item), "History payload 'state_changed' is missing.");
                payload = new StateChangedPayloadDto(MapOrderState(item.StateChanged.OldState), MapOrderState(item.StateChanged.NewState));
                break;
            }

            case OrderHistoryItemKind.Unspecified:
            default:
                throw new ArgumentOutOfRangeException(nameof(item), "Unknown or unspecified history kind.");
        }

        return new OrderHistoryItemDto(
            item.OrderHistoryItemId,
            item.OrderId,
            item.CreatedAt.ToDateTimeOffset(),
            MapHistoryKind(item.Kind),
            payload);
    }
}
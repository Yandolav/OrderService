using Domain.Entities;
using Google.Protobuf.WellKnownTypes;

namespace Presentation.Grpc.Mappings;

internal static class GrpcMappings
{
    public static OrderHistoryItem ToGrpc(this OrderHistory orderHistoryItem)
    {
        var item = new OrderHistoryItem
        {
            OrderHistoryItemId = orderHistoryItem.OrderHistoryItemId,
            OrderId = orderHistoryItem.OrderId,
            CreatedAt = Timestamp.FromDateTimeOffset(orderHistoryItem.OrderHistoryItemCreatedAt),
            Kind = orderHistoryItem.OrderHistoryItemKind switch
            {
                Domain.Enums.OrderHistoryItemKind.Created => OrderHistoryItemKind.CreatedItem,
                Domain.Enums.OrderHistoryItemKind.ItemAdded => OrderHistoryItemKind.ItemAdded,
                Domain.Enums.OrderHistoryItemKind.ItemRemoved => OrderHistoryItemKind.ItemRemoved,
                Domain.Enums.OrderHistoryItemKind.StateChanged => OrderHistoryItemKind.StateChanged,
                _ => OrderHistoryItemKind.Unspecified,
            },
        };

        SetPayload(item, orderHistoryItem);

        return item;
    }

    private static void SetPayload(OrderHistoryItem item, OrderHistory orderHistoryItem)
    {
        switch (orderHistoryItem.OrderHistoryItemPayload)
        {
            case Domain.Entities.Payloads.ItemAddedPayload itemAddedPayload:
                item.ItemAdded = new ItemAddedPayload
                {
                    ProductId = itemAddedPayload.ProductId,
                    Quantity = itemAddedPayload.Quantity,
                };
                break;

            case Domain.Entities.Payloads.ItemRemovedPayload itemRemovedPayload:
                item.ItemRemoved = new ItemRemovedPayload
                {
                    ProductId = itemRemovedPayload.ProductId,
                    Quantity = itemRemovedPayload.Quantity,
                };
                break;

            case Domain.Entities.Payloads.OrderCreatedPayload orderCreatedPayload:
                item.OrderCreated = new OrderCreatedPayload
                {
                    CreatedBy = orderCreatedPayload.CreatedBy,
                };
                break;

            case Domain.Entities.Payloads.StateChangedPayload stateChangedPayload:
                item.StateChanged = new StateChangedPayload
                {
                    OldState = MapState(stateChangedPayload.OldState),
                    NewState = MapState(stateChangedPayload.NewState),
                };
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(orderHistoryItem));
        }
    }

    private static OrderState MapState(Domain.Enums.OrderState state)
    {
        return state switch
        {
            Domain.Enums.OrderState.Created => OrderState.Created,
            Domain.Enums.OrderState.Processing => OrderState.Processing,
            Domain.Enums.OrderState.Completed => OrderState.Completed,
            Domain.Enums.OrderState.Cancelled => OrderState.Cancelled,
            _ => OrderState.Unspecified,
        };
    }
}
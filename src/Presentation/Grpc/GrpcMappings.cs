using Core.Domain.Entities;
using Google.Protobuf.WellKnownTypes;

namespace Presentation.Grpc;

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
                Core.Domain.Enums.OrderHistoryItemKind.Created => OrderHistoryItemKind.CreatedItem,
                Core.Domain.Enums.OrderHistoryItemKind.ItemAdded => OrderHistoryItemKind.ItemAdded,
                Core.Domain.Enums.OrderHistoryItemKind.ItemRemoved => OrderHistoryItemKind.ItemRemoved,
                Core.Domain.Enums.OrderHistoryItemKind.StateChanged => OrderHistoryItemKind.StateChanged,
                _ => OrderHistoryItemKind.Unspecified,
            },
        };

        SetPayload(item, orderHistoryItem);

        return item;
    }

    public static Core.Domain.Enums.OrderHistoryItemKind? ToDomain(this OrderHistoryItemKind kind) =>
        kind switch
        {
            OrderHistoryItemKind.Unspecified => null,
            OrderHistoryItemKind.CreatedItem => Core.Domain.Enums.OrderHistoryItemKind.Created,
            OrderHistoryItemKind.ItemAdded => Core.Domain.Enums.OrderHistoryItemKind.ItemAdded,
            OrderHistoryItemKind.ItemRemoved => Core.Domain.Enums.OrderHistoryItemKind.ItemRemoved,
            OrderHistoryItemKind.StateChanged => Core.Domain.Enums.OrderHistoryItemKind.StateChanged,
            _ => null,
        };

    private static void SetPayload(OrderHistoryItem item, OrderHistory orderHistoryItem)
    {
        switch (orderHistoryItem.OrderHistoryItemPayload)
        {
            case Core.Domain.Payloads.ItemAddedPayload itemAddedPayload:
                item.ItemAdded = new ItemAddedPayload
                {
                    ProductId = itemAddedPayload.ProductId,
                    Quantity = itemAddedPayload.Quantity,
                };
                break;

            case Core.Domain.Payloads.ItemRemovedPayload itemRemovedPayload:
                item.ItemRemoved = new ItemRemovedPayload
                {
                    ProductId = itemRemovedPayload.ProductId,
                    Quantity = itemRemovedPayload.Quantity,
                };
                break;

            case Core.Domain.Payloads.OrderCreatedPayload orderCreatedPayload:
                item.OrderCreated = new OrderCreatedPayload
                {
                    CreatedBy = orderCreatedPayload.CreatedBy,
                };
                break;

            case Core.Domain.Payloads.StateChangedPayload stateChangedPayload:
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

    private static OrderState MapState(Core.Domain.Enums.OrderState state)
    {
        return state switch
        {
            Core.Domain.Enums.OrderState.Created => OrderState.Created,
            Core.Domain.Enums.OrderState.Processing => OrderState.Processing,
            Core.Domain.Enums.OrderState.Completed => OrderState.Completed,
            Core.Domain.Enums.OrderState.Cancelled => OrderState.Cancelled,
            _ => OrderState.Unspecified,
        };
    }
}
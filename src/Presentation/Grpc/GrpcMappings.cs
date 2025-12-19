using Core.Model.Entities;
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
                Core.Model.Enums.OrderHistoryItemKind.Created => OrderHistoryItemKind.CreatedItem,
                Core.Model.Enums.OrderHistoryItemKind.ItemAdded => OrderHistoryItemKind.ItemAdded,
                Core.Model.Enums.OrderHistoryItemKind.ItemRemoved => OrderHistoryItemKind.ItemRemoved,
                Core.Model.Enums.OrderHistoryItemKind.StateChanged => OrderHistoryItemKind.StateChanged,
                Core.Model.Enums.OrderHistoryItemKind.ApprovalReceived => OrderHistoryItemKind.ApprovalReceived,
                Core.Model.Enums.OrderHistoryItemKind.PackingStarted => OrderHistoryItemKind.PackingStarted,
                Core.Model.Enums.OrderHistoryItemKind.PackingFinished => OrderHistoryItemKind.PackingFinished,
                Core.Model.Enums.OrderHistoryItemKind.DeliveryStarted => OrderHistoryItemKind.DeliveryStarted,
                Core.Model.Enums.OrderHistoryItemKind.DeliveryFinished => OrderHistoryItemKind.DeliveryFinished,
                _ => OrderHistoryItemKind.Unspecified,
            },
        };

        SetPayload(item, orderHistoryItem);

        return item;
    }

    public static Core.Model.Enums.OrderHistoryItemKind? ToDomain(this OrderHistoryItemKind kind)
    {
        return kind switch
        {
            OrderHistoryItemKind.Unspecified => null,
            OrderHistoryItemKind.CreatedItem => Core.Model.Enums.OrderHistoryItemKind.Created,
            OrderHistoryItemKind.ItemAdded => Core.Model.Enums.OrderHistoryItemKind.ItemAdded,
            OrderHistoryItemKind.ItemRemoved => Core.Model.Enums.OrderHistoryItemKind.ItemRemoved,
            OrderHistoryItemKind.StateChanged => Core.Model.Enums.OrderHistoryItemKind.StateChanged,
            OrderHistoryItemKind.ApprovalReceived => Core.Model.Enums.OrderHistoryItemKind.ApprovalReceived,
            OrderHistoryItemKind.PackingStarted => Core.Model.Enums.OrderHistoryItemKind.PackingStarted,
            OrderHistoryItemKind.PackingFinished => Core.Model.Enums.OrderHistoryItemKind.PackingFinished,
            OrderHistoryItemKind.DeliveryStarted => Core.Model.Enums.OrderHistoryItemKind.DeliveryStarted,
            OrderHistoryItemKind.DeliveryFinished => Core.Model.Enums.OrderHistoryItemKind.DeliveryFinished,
            _ => null,
        };
    }

    private static void SetPayload(OrderHistoryItem item, OrderHistory orderHistoryItem)
    {
        switch (orderHistoryItem.OrderHistoryItemPayload)
        {
            case Core.Model.Payloads.ItemAddedPayload itemAddedPayload:
                item.ItemAdded = new ItemAddedPayload
                {
                    ProductId = itemAddedPayload.ProductId,
                    Quantity = itemAddedPayload.Quantity,
                };
                break;

            case Core.Model.Payloads.ItemRemovedPayload itemRemovedPayload:
                item.ItemRemoved = new ItemRemovedPayload
                {
                    ProductId = itemRemovedPayload.ProductId,
                    Quantity = itemRemovedPayload.Quantity,
                };
                break;

            case Core.Model.Payloads.OrderCreatedPayload orderCreatedPayload:
                item.OrderCreated = new OrderCreatedPayload
                {
                    CreatedBy = orderCreatedPayload.CreatedBy,
                };
                break;

            case Core.Model.Payloads.StateChangedPayload stateChangedPayload:
                item.StateChanged = new StateChangedPayload
                {
                    OldState = MapState(stateChangedPayload.OldState),
                    NewState = MapState(stateChangedPayload.NewState),
                };
                break;

            case Core.Model.Payloads.ApprovalResultPayload approvalResultPayload:
                item.ApprovalResult = new ApprovalResultPayload
                {
                    IsApproved = approvalResultPayload.IsApproved,
                    CreatedBy = approvalResultPayload.CreatedBy,
                    CreatedAt = approvalResultPayload.CreatedAt.ToTimestamp(),
                };
                break;

            case Core.Model.Payloads.PackingStartedPayload packingStartedPayload:
                item.PackingStarted = new PackingStartedPayload
                {
                    PackingBy = packingStartedPayload.PackingBy,
                    StartedAt = packingStartedPayload.StartedAt.ToTimestamp(),
                };
                break;

            case Core.Model.Payloads.PackingFinishedPayload packingFinishedPayload:
                item.PackingFinished = new PackingFinishedPayload
                {
                    FinishedAt = packingFinishedPayload.FinishedAt.ToTimestamp(),
                    IsFinishedSuccessfully = packingFinishedPayload.IsSuccessful,
                    FailureReason = packingFinishedPayload.FailureReason,
                };
                break;

            case Core.Model.Payloads.DeliveryStartedPayload deliveryStartedPayload:
                item.DeliveryStarted = new DeliveryStartedPayload
                {
                    DeliveredBy = deliveryStartedPayload.DeliveredBy,
                    StartedAt = deliveryStartedPayload.StartedAt.ToTimestamp(),
                };
                break;

            case Core.Model.Payloads.DeliveryFinishedPayload deliveryFinishedPayload:
                item.DeliveryFinished = new DeliveryFinishedPayload
                {
                    FinishedAt = deliveryFinishedPayload.FinishedAt.ToTimestamp(),
                    IsFinishedSuccessfully = deliveryFinishedPayload.IsSuccessful,
                    FailureReason = deliveryFinishedPayload.FailureReason,
                };
                break;
        }
    }

    private static OrderState MapState(Core.Model.Enums.OrderState state)
    {
        return state switch
        {
            Core.Model.Enums.OrderState.Created => OrderState.Created,
            Core.Model.Enums.OrderState.Processing => OrderState.Processing,
            Core.Model.Enums.OrderState.Completed => OrderState.Completed,
            Core.Model.Enums.OrderState.Cancelled => OrderState.Cancelled,
            _ => OrderState.Unspecified,
        };
    }
}
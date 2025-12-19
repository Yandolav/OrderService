using Core.Contracts.Orders;
using Core.Model.Entities;
using Core.Model.Pagination;
using Grpc.Core;

namespace Presentation.Grpc;

public sealed class OrdersGrpcService : OrderService.OrderServiceBase
{
    private readonly IOrdersService _orders;

    public OrdersGrpcService(IOrdersService orders)
    {
        _orders = orders;
    }

    public override async Task<CreateOrderResponse> CreateOrder(CreateOrderRequest request, ServerCallContext context)
    {
        long id = await _orders.CreateOrderAsync(request.CreatedBy, context.CancellationToken);
        return new CreateOrderResponse { OrderId = id };
    }

    public override async Task<AddOrderItemResponse> AddOrderItem(AddOrderItemRequest request, ServerCallContext context)
    {
        long id = await _orders.AddOrderItemAsync(request.OrderId, request.ProductId, request.Quantity, context.CancellationToken);
        return new AddOrderItemResponse { OrderItemId = id };
    }

    public override async Task<RemoveOrderItemResponse> RemoveOrderItem(RemoveOrderItemRequest request, ServerCallContext context)
    {
        bool deleted = await _orders.RemoveOrderItemAsync(request.OrderItemId, context.CancellationToken);
        return new RemoveOrderItemResponse { Deleted = deleted };
    }

    public override async Task<ChangeStateResponse> StartProcessing(ChangeStateRequest request, ServerCallContext context)
    {
        bool updated = await _orders.StartProcessingAsync(request.OrderId, context.CancellationToken);
        return new ChangeStateResponse { Updated = updated };
    }

    public override async Task<ChangeStateResponse> Complete(ChangeStateRequest request, ServerCallContext context)
    {
        bool updated = await _orders.CompleteAsync(request.OrderId, context.CancellationToken);
        return new ChangeStateResponse { Updated = updated };
    }

    public override async Task<ChangeStateResponse> Cancel(ChangeStateRequest request, ServerCallContext context)
    {
        bool updated = await _orders.CancelAsync(request.OrderId, context.CancellationToken);
        return new ChangeStateResponse { Updated = updated };
    }

    public override async Task<GetOrderHistoryResponse> GetOrderHistory(GetOrderHistoryRequest request, ServerCallContext context)
    {
        var response = new GetOrderHistoryResponse();
        await foreach (OrderHistory item in _orders.GetOrderHistoryAsync(request.OrderIds.ToArray(), request.Kind.ToDomain(), new Paging(request.Limit, request.Cursor), context.CancellationToken))
        {
            response.Items.Add(item.ToGrpc());
        }

        return response;
    }

    public override async Task GetOrderHistoryStream(GetOrderHistoryRequest request, IServerStreamWriter<OrderHistoryItem> responseStream, ServerCallContext context)
    {
        await foreach (OrderHistory item in _orders.GetOrderHistoryAsync(request.OrderIds.ToArray(), request.Kind.ToDomain(), new Paging(request.Limit, request.Cursor), context.CancellationToken))
        {
            await responseStream.WriteAsync(item.ToGrpc());
        }
    }
}
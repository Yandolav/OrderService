using HttpGateway.Mappings;
using HttpGateway.Models;
using HttpGateway.Models.AddOrderItem;
using HttpGateway.Models.CreateOrder;
using HttpGateway.Models.OrderHistory;
using Presentation.Grpc;

namespace HttpGateway.Services;

public sealed class OrdersGrpcGateway : IOrdersGrpcGateway
{
    private readonly OrderService.OrderServiceClient _client;
    private readonly GrpcMapper _mapper;

    public OrdersGrpcGateway(OrderService.OrderServiceClient client, GrpcMapper mapper)
    {
        _client = client;
        _mapper = mapper;
    }

    public async Task<CreateOrderResponseDto> CreateOrderAsync(
        CreateOrderRequestDto request,
        CancellationToken cancellationToken)
    {
        var grpcRequest = new CreateOrderRequest
        {
            CreatedBy = request.CreatedBy,
        };

        CreateOrderResponse grpcResponse =
            await _client.CreateOrderAsync(grpcRequest, cancellationToken: cancellationToken);

        return new CreateOrderResponseDto(grpcResponse.OrderId);
    }

    public async Task<AddOrderItemResponseDto> AddOrderItemAsync(
        long orderId,
        AddOrderItemRequestDto request,
        CancellationToken cancellationToken)
    {
        var grpcRequest = new AddOrderItemRequest
        {
            OrderId = orderId,
            ProductId = request.ProductId,
            Quantity = request.Quantity,
        };

        AddOrderItemResponse grpcResponse =
            await _client.AddOrderItemAsync(grpcRequest, cancellationToken: cancellationToken);

        return new AddOrderItemResponseDto(grpcResponse.OrderItemId);
    }

    public async Task<RemoveOrderItemResponseDto> RemoveOrderItemAsync(
        long orderItemId,
        CancellationToken cancellationToken)
    {
        var grpcRequest = new RemoveOrderItemRequest
        {
            OrderItemId = orderItemId,
        };

        RemoveOrderItemResponse grpcResponse =
            await _client.RemoveOrderItemAsync(grpcRequest, cancellationToken: cancellationToken);

        return new RemoveOrderItemResponseDto(grpcResponse.Deleted);
    }

    public async Task<ChangeStateResponseDto> StartProcessingAsync(
        long orderId,
        CancellationToken cancellationToken)
    {
        var grpcRequest = new ChangeStateRequest
        {
            OrderId = orderId,
        };

        ChangeStateResponse grpcResponse =
            await _client.StartProcessingAsync(grpcRequest, cancellationToken: cancellationToken);

        return new ChangeStateResponseDto(grpcResponse.Updated);
    }

    public async Task<ChangeStateResponseDto> CompleteAsync(
        long orderId,
        CancellationToken cancellationToken)
    {
        var grpcRequest = new ChangeStateRequest
        {
            OrderId = orderId,
        };

        ChangeStateResponse grpcResponse =
            await _client.CompleteAsync(grpcRequest, cancellationToken: cancellationToken);

        return new ChangeStateResponseDto(grpcResponse.Updated);
    }

    public async Task<ChangeStateResponseDto> CancelAsync(
        long orderId,
        CancellationToken cancellationToken)
    {
        var grpcRequest = new ChangeStateRequest
        {
            OrderId = orderId,
        };

        ChangeStateResponse grpcResponse =
            await _client.CancelAsync(grpcRequest, cancellationToken: cancellationToken);

        return new ChangeStateResponseDto(grpcResponse.Updated);
    }

    public async Task<GetOrderHistoryResponseDto> GetHistoryAsync(
        long[] orderIds,
        OrderHistoryItemKindDto? kind,
        int limit,
        long cursor,
        CancellationToken cancellationToken)
    {
        var grpcRequest = new GetOrderHistoryRequest
        {
            Limit = limit,
            Cursor = cursor,
        };

        grpcRequest.OrderIds.AddRange(orderIds);

        if (kind.HasValue)
        {
            grpcRequest.Kind = _mapper.MapHistoryKindToGrpc(kind);
        }

        GetOrderHistoryResponse grpcResponse = await _client.GetOrderHistoryAsync(grpcRequest, cancellationToken: cancellationToken);

        var items = grpcResponse.Items.Select(_mapper.MapHistoryItem).ToList();

        return new GetOrderHistoryResponseDto(items);
    }
}
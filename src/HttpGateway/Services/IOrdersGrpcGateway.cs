using HttpGateway.Models;
using HttpGateway.Models.AddOrderItem;
using HttpGateway.Models.CreateOrder;
using HttpGateway.Models.OrderHistory;

namespace HttpGateway.Services;

public interface IOrdersGrpcGateway
{
    Task<CreateOrderResponseDto> CreateOrderAsync(
        CreateOrderRequestDto request,
        CancellationToken cancellationToken);

    Task<AddOrderItemResponseDto> AddOrderItemAsync(
        long orderId,
        AddOrderItemRequestDto request,
        CancellationToken cancellationToken);

    Task<RemoveOrderItemResponseDto> RemoveOrderItemAsync(
        long orderItemId,
        CancellationToken cancellationToken);

    Task<ChangeStateResponseDto> StartProcessingAsync(
        long orderId,
        CancellationToken cancellationToken);

    Task<ChangeStateResponseDto> CompleteAsync(
        long orderId,
        CancellationToken cancellationToken);

    Task<ChangeStateResponseDto> CancelAsync(
        long orderId,
        CancellationToken cancellationToken);

    Task<GetOrderHistoryResponseDto> GetHistoryAsync(
        long[] orderIds,
        OrderHistoryItemKindDto? kind,
        int limit,
        long cursor,
        CancellationToken cancellationToken);
}
using HttpGateway.Models.OrderProcessing;

namespace HttpGateway.Services.OrderProcessingService;

public interface IOrdersProcessingService
{
    Task ApproveOrderAsync(long orderId, ApproveOrderRequestDto request, CancellationToken cancellationToken);

    Task StartOrderPackingAsync(long orderId, StartOrderPackingRequestDto request, CancellationToken cancellationToken);

    Task FinishOrderPackingAsync(long orderId, FinishOrderPackingRequestDto request, CancellationToken cancellationToken);

    Task StartOrderDeliveryAsync(long orderId, StartOrderDeliveryRequestDto request, CancellationToken cancellationToken);

    Task FinishOrderDeliveryAsync(long orderId, FinishOrderDeliveryRequestDto request, CancellationToken cancellationToken);
}
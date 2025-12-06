using HttpGateway.Models.OrderProcessing;
using Orders.ProcessingService.Contracts;
using OrdersProcessingOrderService = Orders.ProcessingService.Contracts.OrderService;

namespace HttpGateway.Services.OrderProcessingService;

public sealed class OrdersProcessingService : IOrdersProcessingService
{
    private readonly OrdersProcessingOrderService.OrderServiceClient _client;

    public OrdersProcessingService(OrdersProcessingOrderService.OrderServiceClient client)
    {
        _client = client;
    }

    public async Task ApproveOrderAsync(
        long orderId,
        ApproveOrderRequestDto request,
        CancellationToken cancellationToken)
    {
        var grpcRequest = new ApproveOrderRequest
        {
            OrderId = orderId,
            IsApproved = request.IsApproved,
            ApprovedBy = request.ApprovedBy,
            FailureReason = request.FailureReason,
        };

        await _client.ApproveOrderAsync(grpcRequest, cancellationToken: cancellationToken);
    }

    public async Task StartOrderPackingAsync(
        long orderId,
        StartOrderPackingRequestDto request,
        CancellationToken cancellationToken)
    {
        var grpcRequest = new StartOrderPackingRequest
        {
            OrderId = orderId,
            PackingBy = request.PackingBy,
        };

        await _client.StartOrderPackingAsync(grpcRequest, cancellationToken: cancellationToken);
    }

    public async Task FinishOrderPackingAsync(
        long orderId,
        FinishOrderPackingRequestDto request,
        CancellationToken cancellationToken)
    {
        var grpcRequest = new FinishOrderPackingRequest
        {
            OrderId = orderId,
            IsSuccessful = request.IsSuccessful,
            FailureReason = request.FailureReason,
        };

        await _client.FinishOrderPackingAsync(grpcRequest, cancellationToken: cancellationToken);
    }

    public async Task StartOrderDeliveryAsync(
        long orderId,
        StartOrderDeliveryRequestDto request,
        CancellationToken cancellationToken)
    {
        var grpcRequest = new StartOrderDeliveryRequest
        {
            OrderId = orderId,
            DeliveredBy = request.DeliveredBy,
        };

        await _client.StartOrderDeliveryAsync(grpcRequest, cancellationToken: cancellationToken);
    }

    public async Task FinishOrderDeliveryAsync(
        long orderId,
        FinishOrderDeliveryRequestDto request,
        CancellationToken cancellationToken)
    {
        var grpcRequest = new FinishOrderDeliveryRequest
        {
            OrderId = orderId,
            IsSuccessful = request.IsSuccessful,
            FailureReason = request.FailureReason,
        };

        await _client.FinishOrderDeliveryAsync(grpcRequest, cancellationToken: cancellationToken);
    }
}
using HttpGateway.Models;
using HttpGateway.Models.AddOrderItem;
using HttpGateway.Models.CreateOrder;
using HttpGateway.Models.OrderHistory;
using HttpGateway.Services;
using Microsoft.AspNetCore.Mvc;

namespace HttpGateway.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class OrdersController : ControllerBase
{
    private readonly IOrdersGrpcGateway _ordersGateway;

    public OrdersController(IOrdersGrpcGateway ordersGateway)
    {
        _ordersGateway = ordersGateway;
    }

    [HttpPost]
    [ProducesResponseType(typeof(CreateOrderResponseDto), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    [ProducesResponseType(typeof(ErrorResponse), 500)]
    public async Task<ActionResult<CreateOrderResponseDto>> Create(
        [FromBody] CreateOrderRequestDto request,
        CancellationToken cancellationToken)
    {
        CreateOrderResponseDto result = await _ordersGateway.CreateOrderAsync(request, cancellationToken);
        return Ok(result);
    }

    [HttpPost("{orderId:long}/items")]
    [ProducesResponseType(typeof(AddOrderItemResponseDto), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    [ProducesResponseType(typeof(ErrorResponse), 404)]
    [ProducesResponseType(typeof(ErrorResponse), 409)]
    [ProducesResponseType(typeof(ErrorResponse), 500)]
    public async Task<ActionResult<AddOrderItemResponseDto>> AddItem(
        long orderId,
        [FromBody] AddOrderItemRequestDto request,
        CancellationToken cancellationToken)
    {
        AddOrderItemResponseDto result = await _ordersGateway.AddOrderItemAsync(orderId, request, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("/api/order-items/{orderItemId:long}")]
    [ProducesResponseType(typeof(RemoveOrderItemResponseDto), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 404)]
    [ProducesResponseType(typeof(ErrorResponse), 409)]
    [ProducesResponseType(typeof(ErrorResponse), 500)]
    public async Task<ActionResult<RemoveOrderItemResponseDto>> RemoveItem(
        long orderItemId,
        CancellationToken cancellationToken)
    {
        RemoveOrderItemResponseDto result = await _ordersGateway.RemoveOrderItemAsync(orderItemId, cancellationToken);
        return Ok(result);
    }

    [HttpPost("{orderId:long}/start-processing")]
    [ProducesResponseType(typeof(ChangeStateResponseDto), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 404)]
    [ProducesResponseType(typeof(ErrorResponse), 409)]
    [ProducesResponseType(typeof(ErrorResponse), 500)]
    public async Task<ActionResult<ChangeStateResponseDto>> StartProcessing(
        long orderId,
        CancellationToken cancellationToken)
    {
        ChangeStateResponseDto result = await _ordersGateway.StartProcessingAsync(orderId, cancellationToken);
        return Ok(result);
    }

    [HttpPost("{orderId:long}/complete")]
    [ProducesResponseType(typeof(ChangeStateResponseDto), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 404)]
    [ProducesResponseType(typeof(ErrorResponse), 409)]
    [ProducesResponseType(typeof(ErrorResponse), 500)]
    public async Task<ActionResult<ChangeStateResponseDto>> Complete(
        long orderId,
        CancellationToken cancellationToken)
    {
        ChangeStateResponseDto result = await _ordersGateway.CompleteAsync(orderId, cancellationToken);
        return Ok(result);
    }

    [HttpPost("{orderId:long}/cancel")]
    [ProducesResponseType(typeof(ChangeStateResponseDto), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 404)]
    [ProducesResponseType(typeof(ErrorResponse), 409)]
    [ProducesResponseType(typeof(ErrorResponse), 500)]
    public async Task<ActionResult<ChangeStateResponseDto>> Cancel(
        long orderId,
        CancellationToken cancellationToken)
    {
        ChangeStateResponseDto result = await _ordersGateway.CancelAsync(orderId, cancellationToken);
        return Ok(result);
    }

    [HttpGet("history")]
    [ProducesResponseType(typeof(GetOrderHistoryResponseDto), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    [ProducesResponseType(typeof(ErrorResponse), 500)]
    public async Task<ActionResult<GetOrderHistoryResponseDto>> GetHistory(
        [FromQuery(Name = "orderIds")] long[] orderIds,
        [FromQuery] OrderHistoryItemKindDto? kind,
        [FromQuery] int limit,
        [FromQuery] long cursor,
        CancellationToken cancellationToken)
    {
        GetOrderHistoryResponseDto result = await _ordersGateway.GetHistoryAsync(orderIds, kind, limit, cursor, cancellationToken);
        return Ok(result);
    }
}

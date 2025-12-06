using HttpGateway.Models;
using HttpGateway.Models.AddOrderItem;
using HttpGateway.Models.CreateOrder;
using HttpGateway.Models.OrderHistory;
using HttpGateway.Services.OrderService;
using Microsoft.AspNetCore.Mvc;

namespace HttpGateway.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class OrdersController : ControllerBase
{
    private readonly IOrdersService _ordersService;

    public OrdersController(IOrdersService ordersService)
    {
        _ordersService = ordersService;
    }

    [HttpPost]
    [ProducesResponseType(typeof(CreateOrderResponseDto), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    [ProducesResponseType(typeof(ErrorResponse), 500)]
    public async Task<ActionResult<CreateOrderResponseDto>> Create(
        [FromBody] CreateOrderRequestDto request,
        CancellationToken cancellationToken)
    {
        CreateOrderResponseDto result = await _ordersService.CreateOrderAsync(request, cancellationToken);
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
        AddOrderItemResponseDto result = await _ordersService.AddOrderItemAsync(orderId, request, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{orderId:long}/items/{orderItemId:long}")]
    [ProducesResponseType(typeof(RemoveOrderItemResponseDto), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 404)]
    [ProducesResponseType(typeof(ErrorResponse), 409)]
    [ProducesResponseType(typeof(ErrorResponse), 500)]
    public async Task<ActionResult<RemoveOrderItemResponseDto>> RemoveItem(
        long orderItemId,
        CancellationToken cancellationToken)
    {
        RemoveOrderItemResponseDto result = await _ordersService.RemoveOrderItemAsync(orderItemId, cancellationToken);
        return Ok(result);
    }

    [HttpPost("{orderId:long}/processing")]
    [ProducesResponseType(typeof(ChangeStateResponseDto), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 404)]
    [ProducesResponseType(typeof(ErrorResponse), 409)]
    [ProducesResponseType(typeof(ErrorResponse), 500)]
    public async Task<ActionResult<ChangeStateResponseDto>> StartProcessing(
        long orderId,
        CancellationToken cancellationToken)
    {
        ChangeStateResponseDto result = await _ordersService.StartProcessingAsync(orderId, cancellationToken);
        return Ok(result);
    }

    [HttpPost("{orderId:long}/cancellation")]
    [ProducesResponseType(typeof(ChangeStateResponseDto), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 404)]
    [ProducesResponseType(typeof(ErrorResponse), 409)]
    [ProducesResponseType(typeof(ErrorResponse), 500)]
    public async Task<ActionResult<ChangeStateResponseDto>> Cancel(
        long orderId,
        CancellationToken cancellationToken)
    {
        ChangeStateResponseDto result = await _ordersService.CancelAsync(orderId, cancellationToken);
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
        GetOrderHistoryResponseDto result = await _ordersService.GetHistoryAsync(orderIds, kind, limit, cursor, cancellationToken);
        return Ok(result);
    }
}

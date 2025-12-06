using HttpGateway.Models;
using HttpGateway.Models.OrderProcessing;
using HttpGateway.Services.OrderProcessingService;
using Microsoft.AspNetCore.Mvc;

namespace HttpGateway.Controllers;

[ApiController]
[Route("api/[controller]/{orderId:long}")]
public sealed class OrdersProcessingController : ControllerBase
{
    private readonly IOrdersProcessingService _ordersProcessingService;

    public OrdersProcessingController(IOrdersProcessingService ordersProcessingService)
    {
        _ordersProcessingService = ordersProcessingService;
    }

    [HttpPost("approval")]
    [ProducesResponseType(200)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    [ProducesResponseType(typeof(ErrorResponse), 404)]
    [ProducesResponseType(typeof(ErrorResponse), 500)]
    public async Task<IActionResult> Approve(
        long orderId,
        [FromBody] ApproveOrderRequestDto request,
        CancellationToken cancellationToken)
    {
        await _ordersProcessingService.ApproveOrderAsync(orderId, request, cancellationToken);
        return Ok();
    }

    [HttpPost("packing/start")]
    [ProducesResponseType(200)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    [ProducesResponseType(typeof(ErrorResponse), 404)]
    [ProducesResponseType(typeof(ErrorResponse), 500)]
    public async Task<IActionResult> StartPacking(
        long orderId,
        [FromBody] StartOrderPackingRequestDto request,
        CancellationToken cancellationToken)
    {
        await _ordersProcessingService.StartOrderPackingAsync(orderId, request, cancellationToken);
        return Ok();
    }

    [HttpPost("packing/finish")]
    [ProducesResponseType(200)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    [ProducesResponseType(typeof(ErrorResponse), 404)]
    [ProducesResponseType(typeof(ErrorResponse), 500)]
    public async Task<IActionResult> FinishPacking(
        long orderId,
        [FromBody] FinishOrderPackingRequestDto request,
        CancellationToken cancellationToken)
    {
        await _ordersProcessingService.FinishOrderPackingAsync(orderId, request, cancellationToken);
        return Ok();
    }

    [HttpPost("delivery/start")]
    [ProducesResponseType(200)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    [ProducesResponseType(typeof(ErrorResponse), 404)]
    [ProducesResponseType(typeof(ErrorResponse), 500)]
    public async Task<IActionResult> StartDelivery(
        long orderId,
        [FromBody] StartOrderDeliveryRequestDto request,
        CancellationToken cancellationToken)
    {
        await _ordersProcessingService.StartOrderDeliveryAsync(orderId, request, cancellationToken);
        return Ok();
    }

    [HttpPost("delivery/finish")]
    [ProducesResponseType(200)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    [ProducesResponseType(typeof(ErrorResponse), 404)]
    [ProducesResponseType(typeof(ErrorResponse), 500)]
    public async Task<IActionResult> FinishDelivery(
        long orderId,
        [FromBody] FinishOrderDeliveryRequestDto request,
        CancellationToken cancellationToken)
    {
        await _ordersProcessingService.FinishOrderDeliveryAsync(orderId, request, cancellationToken);
        return Ok();
    }
}
using HttpGateway.Mappings;
using HttpGateway.Models;
using HttpGateway.Models.AddOrderItem;
using HttpGateway.Models.CreateOrder;
using HttpGateway.Models.OrderHistory;
using Microsoft.AspNetCore.Mvc;
using Presentation.Grpc;

namespace HttpGateway.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class OrdersController : ControllerBase
{
    private readonly OrderService.OrderServiceClient _client;
    private readonly GrpcMapper _mapper;

    public OrdersController(OrderService.OrderServiceClient client, GrpcMapper mapper)
    {
        _client = client;
        _mapper = mapper;
    }

    [HttpPost]
    [ProducesResponseType(typeof(CreateOrderResponseDto), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    [ProducesResponseType(typeof(ErrorResponse), 500)]
    public async Task<ActionResult<CreateOrderResponseDto>> Create([FromBody] CreateOrderRequestDto request, CancellationToken cancellationToken)
    {
        CreateOrderResponse res = await _client.CreateOrderAsync(new CreateOrderRequest { CreatedBy = request.CreatedBy }, cancellationToken: cancellationToken);

        return Ok(new CreateOrderResponseDto(res.OrderId));
    }

    [HttpPost("{orderId:long}/items")]
    [ProducesResponseType(typeof(AddOrderItemResponseDto), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    [ProducesResponseType(typeof(ErrorResponse), 404)]
    [ProducesResponseType(typeof(ErrorResponse), 409)]
    [ProducesResponseType(typeof(ErrorResponse), 500)]
    public async Task<ActionResult<AddOrderItemResponseDto>> AddItem(long orderId, [FromBody] AddOrderItemRequestDto request, CancellationToken cancellationToken)
    {
        AddOrderItemResponse? res = await _client.AddOrderItemAsync(new AddOrderItemRequest { OrderId = orderId, ProductId = request.ProductId, Quantity = request.Quantity, }, cancellationToken: cancellationToken);

        return Ok(new AddOrderItemResponseDto(res.OrderItemId));
    }

    [HttpDelete("/api/order-items/{orderItemId:long}")]
    [ProducesResponseType(typeof(RemoveOrderItemResponseDto), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 404)]
    [ProducesResponseType(typeof(ErrorResponse), 409)]
    [ProducesResponseType(typeof(ErrorResponse), 500)]
    public async Task<ActionResult<RemoveOrderItemResponseDto>> RemoveItem(long orderItemId, CancellationToken cancellationToken)
    {
        RemoveOrderItemResponse? res = await _client.RemoveOrderItemAsync(new RemoveOrderItemRequest { OrderItemId = orderItemId }, cancellationToken: cancellationToken);

        return Ok(new RemoveOrderItemResponseDto(res.Deleted));
    }

    [HttpPost("{orderId:long}/start-processing")]
    [ProducesResponseType(typeof(ChangeStateResponseDto), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 404)]
    [ProducesResponseType(typeof(ErrorResponse), 409)]
    [ProducesResponseType(typeof(ErrorResponse), 500)]
    public async Task<ActionResult<ChangeStateResponseDto>> StartProcessing(long orderId, CancellationToken cancellationToken)
    {
        ChangeStateResponse? res = await _client.StartProcessingAsync(new ChangeStateRequest { OrderId = orderId }, cancellationToken: cancellationToken);

        return Ok(new ChangeStateResponseDto(res.Updated));
    }

    [HttpPost("{orderId:long}/complete")]
    [ProducesResponseType(typeof(ChangeStateResponseDto), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 404)]
    [ProducesResponseType(typeof(ErrorResponse), 409)]
    [ProducesResponseType(typeof(ErrorResponse), 500)]
    public async Task<ActionResult<ChangeStateResponseDto>> Complete(long orderId, CancellationToken cancellationToken)
    {
        ChangeStateResponse? res = await _client.CompleteAsync(new ChangeStateRequest { OrderId = orderId }, cancellationToken: cancellationToken);

        return Ok(new ChangeStateResponseDto(res.Updated));
    }

    [HttpPost("{orderId:long}/cancel")]
    [ProducesResponseType(typeof(ChangeStateResponseDto), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 404)]
    [ProducesResponseType(typeof(ErrorResponse), 409)]
    [ProducesResponseType(typeof(ErrorResponse), 500)]
    public async Task<ActionResult<ChangeStateResponseDto>> Cancel(long orderId, CancellationToken cancellationToken)
    {
        ChangeStateResponse? res = await _client.CancelAsync(new ChangeStateRequest { OrderId = orderId }, cancellationToken: cancellationToken);

        return Ok(new ChangeStateResponseDto(res.Updated));
    }

    [HttpGet("history")]
    [ProducesResponseType(typeof(GetOrderHistoryResponseDto), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    [ProducesResponseType(typeof(ErrorResponse), 500)]
    public async Task<ActionResult<GetOrderHistoryResponseDto>> GetHistory(
        [FromQuery(Name = "orderIds")] long[] orderIds,
        [FromQuery] OrderHistoryItemKindDto? kind,
        CancellationToken cancellationToken,
        [FromQuery] int limit,
        [FromQuery] long cursor)
    {
        var request = new GetOrderHistoryRequest { Limit = limit, Cursor = cursor };
        request.OrderIds.AddRange(orderIds);
        if (kind.HasValue)
        {
            request.Kind = _mapper.MapHistoryKindToGrpc(kind);
        }

        GetOrderHistoryResponse? res = await _client.GetOrderHistoryAsync(request, cancellationToken: cancellationToken);
        var items = res.Items.Select(_mapper.MapHistoryItem).ToList();

        return Ok(new GetOrderHistoryResponseDto(items));
    }
}
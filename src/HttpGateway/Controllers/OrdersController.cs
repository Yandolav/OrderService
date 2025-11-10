using HttpGateway.GrpcClient;
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
    private readonly IGrpcOrdersClientFactory _factory;
    private readonly GrpcMapper _mapper;

    public OrdersController(IGrpcOrdersClientFactory factory, GrpcMapper mapper)
    {
        _factory = factory;
        _mapper = mapper;
    }

    [HttpPost]
    [ProducesResponseType(typeof(CreateOrderResponseDto), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    [ProducesResponseType(typeof(ErrorResponse), 500)]
    public async Task<ActionResult<CreateOrderResponseDto>> Create([FromBody] CreateOrderRequestDto request, CancellationToken cancellationToken)
    {
        OrderService.OrderServiceClient client = _factory.Create();
        CreateOrderResponse res = await client.CreateOrderAsync(new CreateOrderRequest { CreatedBy = request.CreatedBy }, cancellationToken: cancellationToken);

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
        OrderService.OrderServiceClient client = _factory.Create();
        AddOrderItemResponse? res = await client.AddOrderItemAsync(new AddOrderItemRequest { OrderId = orderId, ProductId = request.ProductId, Quantity = request.Quantity, }, cancellationToken: cancellationToken);

        return Ok(new AddOrderItemResponseDto(res.OrderItemId));
    }

    [HttpDelete("/api/order-items/{orderItemId:long}")]
    [ProducesResponseType(typeof(RemoveOrderItemResponseDto), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 404)]
    [ProducesResponseType(typeof(ErrorResponse), 409)]
    [ProducesResponseType(typeof(ErrorResponse), 500)]
    public async Task<ActionResult<RemoveOrderItemResponseDto>> RemoveItem(long orderItemId, CancellationToken cancellationToken)
    {
        OrderService.OrderServiceClient client = _factory.Create();
        RemoveOrderItemResponse? res = await client.RemoveOrderItemAsync(new RemoveOrderItemRequest { OrderItemId = orderItemId }, cancellationToken: cancellationToken);

        return Ok(new RemoveOrderItemResponseDto(res.Deleted));
    }

    [HttpPost("{orderId:long}/start-processing")]
    [ProducesResponseType(typeof(ChangeStateResponseDto), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 404)]
    [ProducesResponseType(typeof(ErrorResponse), 409)]
    [ProducesResponseType(typeof(ErrorResponse), 500)]
    public async Task<ActionResult<ChangeStateResponseDto>> StartProcessing(long orderId, CancellationToken cancellationToken)
    {
        OrderService.OrderServiceClient client = _factory.Create();
        ChangeStateResponse? res = await client.StartProcessingAsync(new ChangeStateRequest { OrderId = orderId }, cancellationToken: cancellationToken);

        return Ok(new ChangeStateResponseDto(res.Updated));
    }

    [HttpPost("{orderId:long}/complete")]
    [ProducesResponseType(typeof(ChangeStateResponseDto), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 404)]
    [ProducesResponseType(typeof(ErrorResponse), 409)]
    [ProducesResponseType(typeof(ErrorResponse), 500)]
    public async Task<ActionResult<ChangeStateResponseDto>> Complete(long orderId, CancellationToken cancellationToken)
    {
        OrderService.OrderServiceClient client = _factory.Create();
        ChangeStateResponse? res = await client.CompleteAsync(new ChangeStateRequest { OrderId = orderId }, cancellationToken: cancellationToken);

        return Ok(new ChangeStateResponseDto(res.Updated));
    }

    [HttpPost("{orderId:long}/cancel")]
    [ProducesResponseType(typeof(ChangeStateResponseDto), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 404)]
    [ProducesResponseType(typeof(ErrorResponse), 409)]
    [ProducesResponseType(typeof(ErrorResponse), 500)]
    public async Task<ActionResult<ChangeStateResponseDto>> Cancel(long orderId, CancellationToken cancellationToken)
    {
        OrderService.OrderServiceClient client = _factory.Create();
        ChangeStateResponse? res = await client.CancelAsync(new ChangeStateRequest { OrderId = orderId }, cancellationToken: cancellationToken);

        return Ok(new ChangeStateResponseDto(res.Updated));
    }

    [HttpGet("history")]
    [ProducesResponseType(typeof(GetOrderHistoryResponseDto), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    [ProducesResponseType(typeof(ErrorResponse), 404)]
    [ProducesResponseType(typeof(ErrorResponse), 500)]
    public async Task<ActionResult<GetOrderHistoryResponseDto>> GetHistory(
        [FromQuery(Name = "orderIds")] long[]? orderIds,
        [FromQuery] OrderHistoryItemKindDto? kind,
        CancellationToken cancellationToken,
        [FromQuery] int limit = 50,
        [FromQuery] long cursor = 0)
    {
        OrderService.OrderServiceClient client = _factory.Create();
        var request = new GetOrderHistoryRequest { Limit = limit, Cursor = cursor };
        if (orderIds is not null && orderIds.Length > 0)
        {
            request.OrderIds.AddRange(orderIds);
        }

        if (kind.HasValue)
        {
            request.Kind = kind.Value switch
            {
                OrderHistoryItemKindDto.Unspecified => OrderHistoryItemKind.Unspecified,
                OrderHistoryItemKindDto.CreatedItem => OrderHistoryItemKind.CreatedItem,
                OrderHistoryItemKindDto.ItemAdded => OrderHistoryItemKind.ItemAdded,
                OrderHistoryItemKindDto.ItemRemoved => OrderHistoryItemKind.ItemRemoved,
                OrderHistoryItemKindDto.StateChanged => OrderHistoryItemKind.StateChanged,
                _ => throw new ArgumentOutOfRangeException(paramName: nameof(kind)),
            };
        }

        GetOrderHistoryResponse? res = await client.GetOrderHistoryAsync(request, cancellationToken: cancellationToken);
        var items = res.Items.Select(_mapper.MapHistoryItem).ToList();

        return Ok(new GetOrderHistoryResponseDto(items));
    }
}
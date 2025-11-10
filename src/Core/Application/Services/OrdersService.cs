using Application.Errors;
using Application.ServiceInterfaces;
using Domain.Entities;
using Domain.Entities.Filters;
using Domain.Entities.Pagination;
using Domain.Entities.Payloads;
using Domain.Enums;
using Domain.Repositories;

namespace Application.Services;

public sealed class OrdersService : IOrdersService
{
    private readonly IOrdersRepository _ordersRepository;
    private readonly IOrderItemsRepository _ordersItemsRepository;
    private readonly IOrderHistoryRepository _ordersHistoryRepository;
    private readonly ITransactionManager _transactionManager;

    public OrdersService(IOrdersRepository ordersRepository, IOrderItemsRepository ordersItemsRepository, IOrderHistoryRepository ordersHistoryRepository, ITransactionManager transactionManager)
    {
        ArgumentNullException.ThrowIfNull(ordersRepository);
        ArgumentNullException.ThrowIfNull(ordersItemsRepository);
        ArgumentNullException.ThrowIfNull(ordersHistoryRepository);
        ArgumentNullException.ThrowIfNull(transactionManager);

        _ordersRepository = ordersRepository;
        _ordersItemsRepository = ordersItemsRepository;
        _ordersHistoryRepository = ordersHistoryRepository;
        _transactionManager = transactionManager;
    }

    public async Task<long> CreateOrderAsync(string createdBy, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(createdBy)) throw new InvalidArgumentAppException("createdBy is required");

        await using ITransaction transaction = await _transactionManager.BeginAsync(cancellationToken);
        DateTimeOffset now = DateTimeOffset.UtcNow;
        long orderId = await _ordersRepository.CreateAsync(createdBy, now, transaction, cancellationToken);
        await _ordersHistoryRepository.CreateAsync(orderId, now, OrderHistoryItemKind.Created, new OrderCreatedPayload(createdBy), transaction, cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return orderId;
    }

    public async Task<long> AddOrderItemAsync(long orderId, long productId, int quantity, CancellationToken cancellationToken)
    {
        if (quantity <= 0) throw new InvalidArgumentAppException("quantity must be > 0");

        Order order = await _ordersRepository.GetByIdAsync(orderId, null, cancellationToken) ?? throw new NotFoundAppException("Order", orderId);
        if (order.OrderState != OrderState.Created)
        {
            throw new ForbiddenForStateAppException("add_items", order.OrderState.ToString());
        }

        await using ITransaction transaction = await _transactionManager.BeginAsync(cancellationToken);
        long itemId = await _ordersItemsRepository.CreateAsync(orderId, productId, quantity, transaction, cancellationToken);
        DateTimeOffset now = DateTimeOffset.UtcNow;
        await _ordersHistoryRepository.CreateAsync(orderId, now, OrderHistoryItemKind.ItemAdded, new ItemAddedPayload(productId, quantity), transaction, cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return itemId;
    }

    public async Task<bool> RemoveOrderItemAsync(long orderItemId, CancellationToken cancellationToken)
    {
        OrderItem item = await _ordersItemsRepository.GetByIdAsync(orderItemId, null, cancellationToken) ?? throw new NotFoundAppException("OrderItem", orderItemId);
        Order order = await _ordersRepository.GetByIdAsync(item.OrderId, null, cancellationToken) ?? throw new NotFoundAppException("Order", item.OrderId);
        if (order.OrderState != OrderState.Created)
        {
            throw new ForbiddenForStateAppException("remove_items", order.OrderState.ToString());
        }

        await using ITransaction transaction = await _transactionManager.BeginAsync(cancellationToken);
        bool deleted = await _ordersItemsRepository.SoftDeleteAsync(orderItemId, transaction, cancellationToken);
        if (deleted)
        {
            DateTimeOffset now = DateTimeOffset.UtcNow;
            await _ordersHistoryRepository.CreateAsync(order.OrderId, now, OrderHistoryItemKind.ItemRemoved, new ItemRemovedPayload(item.ProductId, item.OrderItemQuantity), transaction, cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }

        return deleted;
    }

    public async Task<bool> StartProcessingAsync(long orderId, CancellationToken cancellationToken)
    {
        return await ChangeState(orderId, OrderState.Processing, cancellationToken);
    }

    public async Task<bool> CompleteAsync(long orderId, CancellationToken cancellationToken)
    {
        return await ChangeState(orderId, OrderState.Completed, cancellationToken);
    }

    public async Task<bool> CancelAsync(long orderId, CancellationToken cancellationToken)
    {
        return await ChangeState(orderId, OrderState.Cancelled, cancellationToken);
    }

    public IAsyncEnumerable<OrderHistory> GetOrderHistoryAsync(long[]? orderIds, OrderHistoryItemKind? kind, Paging paging, CancellationToken cancellationToken)
    {
        return _ordersHistoryRepository.SearchAsync(new OrderHistoryFilter(orderIds, kind), paging, null, cancellationToken);
    }

    private async Task<bool> ChangeState(long orderId, OrderState newState, CancellationToken cancellationToken)
    {
        Order order = await _ordersRepository.GetByIdAsync(orderId, null, cancellationToken) ?? throw new NotFoundAppException("Order", orderId);
        if (order.OrderState == newState) throw new InvalidStateAppException("already in requested state", order.OrderState.ToString(), newState.ToString());
        if (order.OrderState is OrderState.Completed or OrderState.Cancelled) throw new InvalidStateAppException("state is terminal", order.OrderState.ToString(), newState.ToString());

        await using ITransaction transaction = await _transactionManager.BeginAsync(cancellationToken);
        bool updated = await _ordersRepository.UpdateStateAsync(orderId, newState, transaction, cancellationToken);
        if (updated)
        {
            DateTimeOffset now = DateTimeOffset.UtcNow;
            await _ordersHistoryRepository.CreateAsync(orderId, now, OrderHistoryItemKind.StateChanged, new StateChangedPayload(order.OrderState, newState), transaction, cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }

        return updated;
    }
}
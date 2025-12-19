using Core.Abstractions.Repositories;
using Core.Contracts.Events;
using Core.Contracts.Orders;
using Core.Model.Entities;
using Core.Model.Enums;
using Core.Model.Errors;
using Core.Model.Filters;
using Core.Model.Pagination;
using Core.Model.Payloads;
using System.Transactions;

namespace Core.Services;

public sealed class OrdersService : IOrdersService
{
    private readonly IOrdersRepository _ordersRepository;
    private readonly IOrderItemsRepository _ordersItemsRepository;
    private readonly IOrderHistoryRepository _ordersHistoryRepository;
    private readonly IOrderEventsProducer _producer;
    private readonly TimeProvider _timeProvider;

    public OrdersService(IOrdersRepository ordersRepository, IOrderItemsRepository ordersItemsRepository, IOrderHistoryRepository ordersHistoryRepository, TimeProvider timeProvider, IOrderEventsProducer producer)
    {
        _ordersRepository = ordersRepository;
        _ordersItemsRepository = ordersItemsRepository;
        _ordersHistoryRepository = ordersHistoryRepository;
        _producer = producer;
        _timeProvider = timeProvider;
    }

    public async Task<long> CreateOrderAsync(string createdBy, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(createdBy)) throw new InvalidArgumentAppException("createdBy is required");

        using var transaction = new TransactionScope(
            TransactionScopeOption.Required,
            new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
            TransactionScopeAsyncFlowOption.Enabled);

        DateTimeOffset now = _timeProvider.GetUtcNow();
        long orderId = await _ordersRepository.CreateAsync(createdBy, now, cancellationToken);
        await _ordersHistoryRepository.CreateAsync(orderId, now, OrderHistoryItemKind.Created, new OrderCreatedPayload(createdBy), cancellationToken);

        transaction.Complete();

        var order = new Order(orderId, OrderState.Created, now, createdBy);
        await _producer.OrderCreatedAsync(order, cancellationToken);
        return orderId;
    }

    public async Task<long> AddOrderItemAsync(long orderId, long productId, int quantity, CancellationToken cancellationToken)
    {
        if (quantity <= 0) throw new InvalidArgumentAppException("quantity must be > 0");

        using var transaction = new TransactionScope(
            TransactionScopeOption.Required,
            new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
            TransactionScopeAsyncFlowOption.Enabled);

        Order order = await _ordersRepository.GetByIdAsync(orderId, cancellationToken) ?? throw new NotFoundAppException("Order", orderId);
        if (order.OrderState != OrderState.Created)
        {
            throw new ForbiddenForStateAppException("add_items", order.OrderState.ToString());
        }

        long itemId = await _ordersItemsRepository.CreateAsync(orderId, productId, quantity, cancellationToken);
        DateTimeOffset now = _timeProvider.GetUtcNow();
        await _ordersHistoryRepository.CreateAsync(orderId, now, OrderHistoryItemKind.ItemAdded, new ItemAddedPayload(productId, quantity), cancellationToken);
        transaction.Complete();
        return itemId;
    }

    public async Task<bool> RemoveOrderItemAsync(long orderItemId, CancellationToken cancellationToken)
    {
        using var transaction = new TransactionScope(
            TransactionScopeOption.Required,
            new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
            TransactionScopeAsyncFlowOption.Enabled);

        OrderItem item = await _ordersItemsRepository.GetByIdAsync(orderItemId, cancellationToken) ?? throw new NotFoundAppException("OrderItem", orderItemId);
        Order order = await _ordersRepository.GetByIdAsync(item.OrderId, cancellationToken) ?? throw new NotFoundAppException("Order", item.OrderId);
        if (order.OrderState != OrderState.Created)
        {
            throw new ForbiddenForStateAppException("remove_items", order.OrderState.ToString());
        }

        bool deleted = await _ordersItemsRepository.SoftDeleteAsync(orderItemId, cancellationToken);
        if (deleted)
        {
            DateTimeOffset now = _timeProvider.GetUtcNow();
            await _ordersHistoryRepository.CreateAsync(order.OrderId, now, OrderHistoryItemKind.ItemRemoved, new ItemRemovedPayload(item.ProductId, item.OrderItemQuantity), cancellationToken);
            transaction.Complete();
        }

        return deleted;
    }

    public async Task<bool> StartProcessingAsync(long orderId, CancellationToken cancellationToken)
    {
        bool started = await ChangeState(orderId, OrderState.Processing, cancellationToken);

        if (started)
        {
            Order order = await _ordersRepository.GetByIdAsync(orderId, cancellationToken) ?? throw new NotFoundAppException("Order", orderId);
            await _producer.OrderProcessingStartedAsync(order, cancellationToken);
        }

        return started;
    }

    public async Task<bool> CompleteAsync(long orderId, CancellationToken cancellationToken)
    {
        return await ChangeState(orderId, OrderState.Completed, cancellationToken);
    }

    public async Task<bool> CancelAsync(long orderId, CancellationToken cancellationToken)
    {
        Order order = await _ordersRepository.GetByIdAsync(orderId, cancellationToken) ?? throw new NotFoundAppException("Order", orderId);
        if (order.OrderState != OrderState.Created) throw new ForbiddenForStateAppException("cancel", order.OrderState.ToString());
        return await ChangeState(orderId, OrderState.Cancelled, cancellationToken);
    }

    public async Task<bool> CancelToFailureAsync(long orderId, CancellationToken cancellationToken)
    {
        return await ChangeState(orderId, OrderState.Cancelled, cancellationToken);
    }

    public IAsyncEnumerable<OrderHistory> GetOrderHistoryAsync(long[] orderIds, OrderHistoryItemKind? kind, Paging paging, CancellationToken cancellationToken)
    {
        return _ordersHistoryRepository.SearchAsync(new OrderHistoryFilter(orderIds, kind), paging, cancellationToken);
    }

    private async Task<bool> ChangeState(long orderId, OrderState newState, CancellationToken cancellationToken)
    {
        using var transaction = new TransactionScope(
            TransactionScopeOption.Required,
            new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
            TransactionScopeAsyncFlowOption.Enabled);

        Order order = await _ordersRepository.GetByIdAsync(orderId, cancellationToken) ?? throw new NotFoundAppException("Order", orderId);
        if (order.OrderState == newState) throw new InvalidStateAppException("already in requested state", order.OrderState.ToString(), newState.ToString());
        if (order.OrderState is OrderState.Completed or OrderState.Cancelled) throw new InvalidStateAppException("state is terminal", order.OrderState.ToString(), newState.ToString());

        bool updated = await _ordersRepository.UpdateStateAsync(orderId, newState, cancellationToken);
        if (updated)
        {
            DateTimeOffset now = _timeProvider.GetUtcNow();
            await _ordersHistoryRepository.CreateAsync(orderId, now, OrderHistoryItemKind.StateChanged, new StateChangedPayload(order.OrderState, newState), cancellationToken);
            transaction.Complete();
        }

        return updated;
    }
}
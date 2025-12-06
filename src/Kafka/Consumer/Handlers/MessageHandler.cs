using Confluent.Kafka;
using Core.Application.Ports.PrimaryPorts;
using Core.Application.Ports.SecondaryPorts;
using Core.Domain.Enums;
using Core.Domain.Payloads;
using Orders.Kafka.Contracts;

namespace Kafka.Consumer.Handlers;

public class MessageHandler : IMessageHandler
{
    private readonly IOrdersService _ordersService;
    private readonly IOrderHistoryRepository _historyRepository;
    private readonly TimeProvider _timeProvider;

    public MessageHandler(IOrdersService ordersService, IOrderHistoryRepository historyRepository, TimeProvider timeProvider)
    {
        _ordersService = ordersService;
        _historyRepository = historyRepository;
        _timeProvider = timeProvider;
    }

    public async Task HandleASync(IEnumerable<ConsumeResult<OrderProcessingKey, OrderProcessingValue>> messages, CancellationToken cancellationToken)
    {
        foreach (ConsumeResult<OrderProcessingKey, OrderProcessingValue> message in messages)
        {
            OrderProcessingValue value = message.Message.Value;
            switch (value.EventCase)
            {
                case OrderProcessingValue.EventOneofCase.ApprovalReceived:
                    await HandleApprovalAsync(value.ApprovalReceived, cancellationToken);
                    break;

                case OrderProcessingValue.EventOneofCase.PackingStarted:
                    await HandlePackingStartedAsync(value.PackingStarted, cancellationToken);
                    break;

                case OrderProcessingValue.EventOneofCase.PackingFinished:
                    await HandlePackingFinishedAsync(value.PackingFinished, cancellationToken);
                    break;

                case OrderProcessingValue.EventOneofCase.DeliveryStarted:
                    await HandleDeliveryStartedAsync(value.DeliveryStarted, cancellationToken);
                    break;

                case OrderProcessingValue.EventOneofCase.DeliveryFinished:
                    await HandleDeliveryFinishedAsync(value.DeliveryFinished, cancellationToken);
                    break;

                case OrderProcessingValue.EventOneofCase.None:
                    break;
            }
        }
    }

    private async Task HandleApprovalAsync(OrderProcessingValue.Types.OrderApprovalReceived eventCase, CancellationToken cancellationToken)
    {
        long orderId = eventCase.OrderId;
        var payload = new ApprovalResultPayload(eventCase.IsApproved, eventCase.CreatedBy, eventCase.CreatedAt.ToDateTimeOffset());
        await _historyRepository.CreateAsync(orderId, _timeProvider.GetUtcNow(), OrderHistoryItemKind.ApprovalReceived, payload, cancellationToken);

        if (!eventCase.IsApproved)
        {
            await _ordersService.CancelToFailureAsync(orderId, cancellationToken);
        }
    }

    private async Task HandlePackingStartedAsync(OrderProcessingValue.Types.OrderPackingStarted eventCase, CancellationToken cancellationToken)
    {
        var payload = new PackingStartedPayload(eventCase.PackingBy, eventCase.StartedAt.ToDateTimeOffset());
        await _historyRepository.CreateAsync(eventCase.OrderId, _timeProvider.GetUtcNow(), OrderHistoryItemKind.PackingStarted, payload, cancellationToken);
    }

    private async Task HandlePackingFinishedAsync(OrderProcessingValue.Types.OrderPackingFinished eventCase, CancellationToken cancellationToken)
    {
        long orderId = eventCase.OrderId;
        var payload = new PackingFinishedPayload(eventCase.FinishedAt.ToDateTimeOffset(), eventCase.IsFinishedSuccessfully, eventCase.FailureReason);
        await _historyRepository.CreateAsync(orderId, _timeProvider.GetUtcNow(), OrderHistoryItemKind.PackingFinished, payload, cancellationToken);

        if (!eventCase.IsFinishedSuccessfully)
        {
            await _ordersService.CancelToFailureAsync(orderId, cancellationToken);
        }
    }

    private async Task HandleDeliveryStartedAsync(OrderProcessingValue.Types.OrderDeliveryStarted eventCase, CancellationToken cancellationToken)
    {
        var payload = new DeliveryStartedPayload(eventCase.DeliveredBy, eventCase.StartedAt.ToDateTimeOffset());
        await _historyRepository.CreateAsync(eventCase.OrderId, _timeProvider.GetUtcNow(), OrderHistoryItemKind.DeliveryStarted, payload, cancellationToken);
    }

    private async Task HandleDeliveryFinishedAsync(OrderProcessingValue.Types.OrderDeliveryFinished eventCase, CancellationToken cancellationToken)
    {
        long orderId = eventCase.OrderId;
        var payload = new DeliveryFinishedPayload(eventCase.FinishedAt.ToDateTimeOffset(), eventCase.IsFinishedSuccessfully, eventCase.FailureReason);
        await _historyRepository.CreateAsync(orderId, _timeProvider.GetUtcNow(), OrderHistoryItemKind.DeliveryFinished, payload, cancellationToken);

        if (!eventCase.IsFinishedSuccessfully)
        {
            await _ordersService.CancelToFailureAsync(orderId, cancellationToken);
            return;
        }

        await _ordersService.CompleteAsync(orderId, cancellationToken);
    }
}
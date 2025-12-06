using System.Text.Json.Serialization;

namespace Core.Domain.Payloads;

[JsonDerivedType(typeof(OrderCreatedPayload), typeDiscriminator: nameof(OrderCreatedPayload))]
[JsonDerivedType(typeof(ItemAddedPayload), typeDiscriminator: nameof(ItemAddedPayload))]
[JsonDerivedType(typeof(ItemRemovedPayload), typeDiscriminator: nameof(ItemRemovedPayload))]
[JsonDerivedType(typeof(StateChangedPayload), typeDiscriminator: nameof(StateChangedPayload))]
[JsonDerivedType(typeof(ApprovalResultPayload), typeDiscriminator: nameof(ApprovalResultPayload))]
[JsonDerivedType(typeof(PackingStartedPayload), typeDiscriminator: nameof(PackingStartedPayload))]
[JsonDerivedType(typeof(PackingFinishedPayload), typeDiscriminator: nameof(PackingFinishedPayload))]
[JsonDerivedType(typeof(DeliveryStartedPayload), typeDiscriminator: nameof(DeliveryStartedPayload))]
[JsonDerivedType(typeof(DeliveryFinishedPayload), typeDiscriminator: nameof(DeliveryFinishedPayload))]
public interface IOrderHistoryPayload;
using System.Text.Json.Serialization;

namespace HttpGateway.Models.Payloads;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(OrderCreatedPayloadDto), typeDiscriminator: "order_created")]
[JsonDerivedType(typeof(ItemAddedPayloadDto), typeDiscriminator: "item_added")]
[JsonDerivedType(typeof(ItemRemovedPayloadDto), typeDiscriminator: "item_removed")]
[JsonDerivedType(typeof(StateChangedPayloadDto), typeDiscriminator: "state_changed")]
[JsonDerivedType(typeof(ApprovalResultPayloadDto), typeDiscriminator: "approval_received")]
[JsonDerivedType(typeof(PackingStartedPayloadDto), typeDiscriminator: "packing_started")]
[JsonDerivedType(typeof(PackingFinishedPayloadDto), typeDiscriminator: "packing_finished")]
[JsonDerivedType(typeof(DeliveryStartedPayloadDto), typeDiscriminator: "delivery_started")]
[JsonDerivedType(typeof(DeliveryFinishedPayloadDto), typeDiscriminator: "delivery_finished")]
public abstract class OrderHistoryItemPayloadDto;
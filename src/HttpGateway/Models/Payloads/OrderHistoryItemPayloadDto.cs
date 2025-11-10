using System.Text.Json.Serialization;

namespace HttpGateway.Models.Payloads;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(OrderCreatedPayloadDto), typeDiscriminator: "order_created")]
[JsonDerivedType(typeof(ItemAddedPayloadDto), typeDiscriminator: "item_added")]
[JsonDerivedType(typeof(ItemRemovedPayloadDto), typeDiscriminator: "item_removed")]
[JsonDerivedType(typeof(StateChangedPayloadDto), typeDiscriminator: "state_changed")]
public abstract class OrderHistoryItemPayloadDto { }
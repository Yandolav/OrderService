namespace HttpGateway.Models.OrderProcessing;

public record FinishOrderDeliveryRequestDto(bool IsSuccessful, string? FailureReason);
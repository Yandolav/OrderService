namespace HttpGateway.Models.OrderProcessing;

public record FinishOrderPackingRequestDto(bool IsSuccessful, string? FailureReason);
namespace HttpGateway.Models.OrderProcessing;

public record ApproveOrderRequestDto(bool IsApproved, string ApprovedBy, string? FailureReason);
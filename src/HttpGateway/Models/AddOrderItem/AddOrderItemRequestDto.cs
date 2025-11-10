using System.ComponentModel.DataAnnotations;

namespace HttpGateway.Models.AddOrderItem;

public sealed class AddOrderItemRequestDto
{
    [Required]
    public long ProductId { get; init; }

    [Range(1, int.MaxValue)]
    public int Quantity { get; init; }
}
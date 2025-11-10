using System.ComponentModel.DataAnnotations;

namespace HttpGateway.Models.CreateOrder;

public sealed class CreateOrderRequestDto
{
    public CreateOrderRequestDto(string createdBy)
    {
        CreatedBy = createdBy;
    }

    [Required]
    [MinLength(1)]
    public string CreatedBy { get; init; }
}
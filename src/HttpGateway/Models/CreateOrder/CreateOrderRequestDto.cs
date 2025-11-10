using System.ComponentModel.DataAnnotations;

namespace Task3.HttpGateway.Models;

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
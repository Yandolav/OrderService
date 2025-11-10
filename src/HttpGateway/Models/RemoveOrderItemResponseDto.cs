namespace HttpGateway.Models;

public sealed class RemoveOrderItemResponseDto
{
    public RemoveOrderItemResponseDto(bool deleted)
    {
        Deleted = deleted;
    }

    public bool Deleted { get; }
}
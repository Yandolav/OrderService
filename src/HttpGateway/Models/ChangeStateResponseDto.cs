namespace Task3.HttpGateway.Models;

public sealed class ChangeStateResponseDto
{
    public ChangeStateResponseDto(bool updated)
    {
        Updated = updated;
    }

    public bool Updated { get; }
}
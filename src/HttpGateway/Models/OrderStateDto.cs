namespace HttpGateway.Models;

public enum OrderStateDto
{
    Unspecified = 0,
    Created = 1,
    Processing = 2,
    Completed = 3,
    Cancelled = 4,
}
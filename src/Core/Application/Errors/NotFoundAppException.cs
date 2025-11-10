namespace Core.Application.Errors;

public sealed class NotFoundAppException : AppException
{
    public NotFoundAppException(string entity, object id) : base(ErrorCodes.NotFound, $"{entity} not found (id={id})") { }
}
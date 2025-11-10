namespace Core.Application.Errors;

public sealed class InvalidArgumentAppException : AppException
{
    public InvalidArgumentAppException(string message) : base(ErrorCodes.InvalidArgument, message) { }
}
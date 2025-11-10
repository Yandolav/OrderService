namespace Application.Errors;

public sealed class InvalidStateAppException : AppException
{
    public InvalidStateAppException(string message, string currentState, string? requestedState = null) : base(ErrorCodes.InvalidState, requestedState is null ? $"{message} (current={currentState})" : $"{message} (current={currentState}, requested={requestedState})") { }
}
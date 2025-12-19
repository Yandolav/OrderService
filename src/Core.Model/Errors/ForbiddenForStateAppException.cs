namespace Core.Model.Errors;

public sealed class ForbiddenForStateAppException : AppException
{
    public ForbiddenForStateAppException(string action, string currentState) : base(ErrorCodes.ForbiddenForState, $"{action} forbidden for current state '{currentState}'") { }
}
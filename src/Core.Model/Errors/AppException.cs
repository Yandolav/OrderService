namespace Core.Model.Errors;

public abstract class AppException : Exception
{
    public ErrorCodes Code { get; }

    protected AppException(ErrorCodes code, string message, Exception? inner = null) : base(message, inner)
    {
        Code = code;
    }
}
namespace Application.Errors;

public static class ErrorCodes
{
    public static string InvalidArgument { get; } = "invalid_argument";

    public static string NotFound { get; } = "not_found";

    public static string ForbiddenForState { get; } = "forbidden_for_state";

    public static string InvalidState { get; } = "invalid_state";

    public static string InternalError { get; } = "internal_error";
}
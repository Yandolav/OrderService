using System.Net;

namespace HttpGateway.Middleware;

public sealed class HttpError
{
    public HttpStatusCode StatusCode { get; }

    public string Code { get; }

    public string Message { get; }

    public HttpError(HttpStatusCode statusCode, string code, string message)
    {
        StatusCode = statusCode;
        Code = code;
        Message = message;
    }
}
using Grpc.Core;
using HttpGateway.Models;
using Microsoft.AspNetCore.Http;
using System.Net;

namespace Task3.HttpGateway.Middleware;

public sealed class GrpcExceptionMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (RpcException ex)
        {
            (HttpStatusCode http, string code, string message) = Map(ex);
            context.Response.StatusCode = (int)http;
            await context.Response.WriteAsJsonAsync(new ErrorResponse(code, message));
        }
        catch
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsJsonAsync(new ErrorResponse("internal_error", "internal error"));
        }
    }

    private static (HttpStatusCode Http, string Code, string Message) Map(RpcException ex)
    {
        string code = ex.Trailers.FirstOrDefault(t => t.Key.Equals("error-code", StringComparison.OrdinalIgnoreCase))?.Value ?? "internal_error";
        string message = ex.Trailers.FirstOrDefault(t => t.Key.Equals("error-message", StringComparison.OrdinalIgnoreCase))?.Value ?? (string.IsNullOrWhiteSpace(ex.Status.Detail) ? "internal error" : ex.Status.Detail);

        HttpStatusCode http = ex.StatusCode switch
        {
            StatusCode.OK => HttpStatusCode.OK,
            StatusCode.Cancelled => (HttpStatusCode)499,
            StatusCode.Unknown => HttpStatusCode.InternalServerError,
            StatusCode.InvalidArgument => HttpStatusCode.BadRequest,
            StatusCode.DeadlineExceeded => HttpStatusCode.GatewayTimeout,
            StatusCode.NotFound => HttpStatusCode.NotFound,
            StatusCode.AlreadyExists => HttpStatusCode.Conflict,
            StatusCode.PermissionDenied => HttpStatusCode.Forbidden,
            StatusCode.ResourceExhausted => HttpStatusCode.TooManyRequests,
            StatusCode.FailedPrecondition => HttpStatusCode.Conflict,
            StatusCode.Aborted => HttpStatusCode.Conflict,
            StatusCode.OutOfRange => HttpStatusCode.BadRequest,
            StatusCode.Unimplemented => HttpStatusCode.NotImplemented,
            StatusCode.Internal => HttpStatusCode.InternalServerError,
            StatusCode.Unavailable => HttpStatusCode.ServiceUnavailable,
            StatusCode.DataLoss => HttpStatusCode.InternalServerError,
            StatusCode.Unauthenticated => HttpStatusCode.Unauthorized,
            _ => HttpStatusCode.InternalServerError,
        };

        return (http, code, message);
    }
}
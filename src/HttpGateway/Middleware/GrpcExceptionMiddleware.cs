using Grpc.Core;
using HttpGateway.Models;
using Microsoft.AspNetCore.Http;
using System.Net;

namespace HttpGateway.Middleware;

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

        HttpStatusCode http;
        if (ex.StatusCode == StatusCode.InvalidArgument)
        {
            http = HttpStatusCode.BadRequest;
        }
        else if (ex.StatusCode == StatusCode.NotFound)
        {
            http = HttpStatusCode.NotFound;
        }
        else if (ex.StatusCode == StatusCode.FailedPrecondition)
        {
            http = HttpStatusCode.Conflict;
        }
        else
        {
            http = HttpStatusCode.InternalServerError;
        }

        return (http, code, message);
    }
}
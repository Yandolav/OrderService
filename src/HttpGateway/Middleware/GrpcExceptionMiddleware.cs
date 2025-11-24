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
            HttpError error = Map(ex);

            context.Response.StatusCode = (int)error.StatusCode;
            await context.Response.WriteAsJsonAsync(new ErrorResponse(error.Code, error.Message));
        }
        catch
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsJsonAsync(new ErrorResponse("internal_error", "internal error"));
        }
    }

    private static HttpError Map(RpcException ex)
    {
        string code = ex.Trailers.FirstOrDefault(t => t.Key.Equals("error-code", StringComparison.OrdinalIgnoreCase))?.Value ?? "internal_error";
        string message = ex.Trailers.FirstOrDefault(t => t.Key.Equals("error-message", StringComparison.OrdinalIgnoreCase))?.Value ?? (string.IsNullOrWhiteSpace(ex.Status.Detail) ? "internal error" : ex.Status.Detail);

        HttpStatusCode httpStatus;
        if (ex.StatusCode == StatusCode.InvalidArgument)
        {
            httpStatus = HttpStatusCode.BadRequest;
        }
        else if (ex.StatusCode == StatusCode.NotFound)
        {
            httpStatus = HttpStatusCode.NotFound;
        }
        else if (ex.StatusCode == StatusCode.FailedPrecondition)
        {
            httpStatus = HttpStatusCode.Conflict;
        }
        else
        {
            httpStatus = HttpStatusCode.InternalServerError;
        }

        return new HttpError(httpStatus, code, message);
    }
}
using Application.Errors;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.Extensions.Logging;

namespace Presentation.Grpc.Interceptors;

public sealed class ErrorHandlingInterceptor : Interceptor
{
    private readonly ILogger<ErrorHandlingInterceptor> _logger;

    public ErrorHandlingInterceptor(ILogger<ErrorHandlingInterceptor> logger)
    {
        _logger = logger;
    }

    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        try
        {
            return await continuation(request, context);
        }
        catch (Exception exception)
        {
            throw BuildRpcException(exception);
        }
    }

    public override async Task ServerStreamingServerHandler<TRequest, TResponse>(
        TRequest request,
        IServerStreamWriter<TResponse> responseStream,
        ServerCallContext context,
        ServerStreamingServerMethod<TRequest, TResponse> continuation)
    {
        try
        {
            await continuation(request, responseStream, context);
        }
        catch (Exception exception)
        {
            throw BuildRpcException(exception);
        }
    }

    private RpcException BuildRpcException(Exception exception)
    {
        (StatusCode status, string code, string message) = MapException(exception);

        _logger.LogWarning(exception, "gRPC error {Code}", code);

        var trailers = new Metadata
        {
            { "error-code", code },
            { "error-message", message },
        };

        return new RpcException(new Status(status, message), trailers);
    }

    private (StatusCode Status, string Code, string Message) MapException(Exception exception)
    {
        switch (exception)
        {
            case AppException appException:
            {
                StatusCode status = appException.Code switch
                {
                    string code when code == ErrorCodes.InvalidArgument => StatusCode.InvalidArgument,
                    string code when code == ErrorCodes.NotFound => StatusCode.NotFound,
                    string code when code == ErrorCodes.ForbiddenForState => StatusCode.FailedPrecondition,
                    string code when code == ErrorCodes.InvalidState => StatusCode.FailedPrecondition,
                    _ => StatusCode.Internal,
                };

                return (status, appException.Code, appException.Message);
            }

            case ArgumentException or ArgumentOutOfRangeException:
                return (StatusCode.InvalidArgument, ErrorCodes.InvalidArgument, exception.Message);

            default:
                return (StatusCode.Internal, ErrorCodes.InternalError, "internal error");
        }
    }
}
using Core.Model.Errors;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.Extensions.Logging;

namespace Presentation.Grpc;

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
        GrpcError error = MapException(exception);

        _logger.LogWarning(exception, "gRPC error {Code}", error.Code);

        var trailers = new Metadata
        {
            { "error-code", ToErrorCodeString(error.Code) },
            { "error-message", error.Message },
        };

        return new RpcException(new Status(error.Status, error.Message), trailers);
    }

    private GrpcError MapException(Exception exception)
    {
        switch (exception)
        {
            case AppException appException:
            {
                StatusCode status = appException.Code switch
                {
                    ErrorCodes.InvalidArgument => StatusCode.InvalidArgument,
                    ErrorCodes.NotFound => StatusCode.NotFound,
                    ErrorCodes.ForbiddenForState => StatusCode.FailedPrecondition,
                    ErrorCodes.InvalidState => StatusCode.FailedPrecondition,
                    ErrorCodes.InternalError => StatusCode.Internal,
                    _ => StatusCode.Internal,
                };

                return new GrpcError(status, appException.Code, appException.Message);
            }

            case ArgumentException or ArgumentOutOfRangeException:
                return new GrpcError(StatusCode.InvalidArgument, ErrorCodes.InvalidArgument, exception.Message);

            default:
                return new GrpcError(StatusCode.Internal, ErrorCodes.InternalError, "internal error");
        }
    }

    private string ToErrorCodeString(ErrorCodes code)
    {
        return code switch
        {
            ErrorCodes.InvalidArgument => "invalid_argument",
            ErrorCodes.NotFound => "not_found",
            ErrorCodes.ForbiddenForState => "forbidden_for_state",
            ErrorCodes.InvalidState => "invalid_state",
            ErrorCodes.InternalError => "internal_error",
            _ => "internal_error",
        };
    }
}
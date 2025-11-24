using Core.Application.Errors;
using Grpc.Core;

namespace Presentation.Grpc;

public sealed class GrpcError
{
    public StatusCode Status { get; }

    public ErrorCodes Code { get; }

    public string Message { get; }

    public GrpcError(StatusCode status, ErrorCodes code, string message)
    {
        Status = status;
        Code = code;
        Message = message;
    }
}

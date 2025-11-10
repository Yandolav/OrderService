using Grpc.Net.Client;
using HttpGateway.Options;
using Microsoft.Extensions.Options;
using Presentation.Grpc;

namespace HttpGateway.GrpcClient;

internal sealed class GrpcOrdersClientFactory : IGrpcOrdersClientFactory
{
    private readonly IOptions<GrpcClientOptions> _options;

    public GrpcOrdersClientFactory(IOptions<GrpcClientOptions> options)
    {
        _options = options;
    }

    public OrderService.OrderServiceClient Create()
    {
        string? address = _options.Value.Url;
        if (string.IsNullOrWhiteSpace(address)) throw new InvalidOperationException("GrpcServer:Address is not configured.");

        var channel = GrpcChannel.ForAddress(address);
        return new OrderService.OrderServiceClient(channel);
    }
}
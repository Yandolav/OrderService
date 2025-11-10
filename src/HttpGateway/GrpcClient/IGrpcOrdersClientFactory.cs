using Presentation.Grpc;

namespace HttpGateway.GrpcClient;

public interface IGrpcOrdersClientFactory
{
    OrderService.OrderServiceClient Create();
}
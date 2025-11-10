using Presentation.Grpc;

namespace Task3.HttpGateway.GrpcClient;

public interface IGrpcOrdersClientFactory
{
    OrderService.OrderServiceClient Create();
}
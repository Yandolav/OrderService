namespace Core.Application.Ports.PrimaryPorts;

public interface IProductsService
{
    Task<long> CreateProductAsync(string name, decimal price, CancellationToken cancellationToken);
}
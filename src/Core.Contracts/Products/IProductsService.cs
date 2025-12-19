namespace Core.Contracts.Products;

public interface IProductsService
{
    Task<long> CreateProductAsync(string name, decimal price, CancellationToken cancellationToken);
}
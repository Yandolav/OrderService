using Core.Application.Errors;
using Core.Application.Ports.PrimaryPorts;
using Core.Application.Ports.SecondaryPorts;

namespace Core.Application.Services;

public sealed class ProductsService : IProductsService
{
    private readonly IProductsRepository _productsRepository;

    public ProductsService(IProductsRepository products)
    {
        ArgumentNullException.ThrowIfNull(products);

        _productsRepository = products;
    }

    public async Task<long> CreateProductAsync(string name, decimal price, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new InvalidArgumentAppException("name is required");
        if (price <= 0) throw new InvalidArgumentAppException("price must be > 0");

        return await _productsRepository.CreateAsync(name, price, null, cancellationToken);
    }
}
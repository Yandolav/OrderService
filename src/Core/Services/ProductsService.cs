using Core.Abstractions.Repositories;
using Core.Contracts.Products;
using Core.Model.Errors;

namespace Core.Services;

public sealed class ProductsService : IProductsService
{
    private readonly IProductsRepository _productsRepository;

    public ProductsService(IProductsRepository products)
    {
        _productsRepository = products;
    }

    public async Task<long> CreateProductAsync(string name, decimal price, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new InvalidArgumentAppException("name is required");
        if (price <= 0) throw new InvalidArgumentAppException("price must be > 0");

        return await _productsRepository.CreateAsync(name, price, cancellationToken);
    }
}
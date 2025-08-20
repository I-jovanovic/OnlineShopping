using OnlineShopping.Core.Entities;

namespace OnlineShopping.Core.Interfaces;

/// <summary>
/// Product service interface
/// </summary>
public interface IProductService : IService
{
    Task<Product> CreateProductAsync(string name, string? description, decimal price, string sku, int stockQuantity, Guid categoryId);
    Task<Product?> GetProductByIdAsync(Guid id);
    Task<Product?> GetProductBySkuAsync(string sku);
    Task<IEnumerable<Product>> GetAllProductsAsync();
    Task<IEnumerable<Product>> GetProductsByCategoryAsync(Guid categoryId);
    Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm);
    Task<Product> UpdateProductAsync(Product product);
    Task DeleteProductAsync(Guid id);
    Task<bool> IsSkuUniqueAsync(string sku, Guid? excludeProductId = null);
    Task UpdateStockAsync(Guid productId, int quantity);
    Task<bool> HasStockAsync(Guid productId, int quantity);
}
using OnlineShopping.Core.Entities;

namespace OnlineShopping.Core.Interfaces;

/// <summary>
/// Product repository interface
/// </summary>
public interface IProductRepository : IRepository<Product>
{
    Task<Product?> GetBySkuAsync(string sku);
    Task<IEnumerable<Product>> GetByCategoryAsync(Guid categoryId);
    Task<IEnumerable<Product>> GetActiveProductsAsync();
    Task<IEnumerable<Product>> SearchAsync(string searchTerm);
    Task<bool> SkuExistsAsync(string sku);
    Task<Product?> GetWithCategoryAsync(Guid productId);
}
using OnlineShopping.Core.DTOs;

namespace OnlineShopping.Core.Interfaces;

/// <summary>
/// Product service interface
/// </summary>
public interface IProductService : IService
{
    Task<ProductDto> CreateProductAsync(CreateProductDto dto);
    Task<ProductDto?> GetProductAsync(Guid productId);
    Task<ProductDto?> GetProductBySkuAsync(string sku);
    Task<IEnumerable<ProductDto>> GetProductsByCategoryAsync(Guid categoryId);
    Task<IEnumerable<ProductDto>> GetActiveProductsAsync();
    Task<IEnumerable<ProductDto>> SearchProductsAsync(string searchTerm);
    Task<ProductDto> UpdateProductAsync(Guid productId, UpdateProductDto dto);
    Task<bool> UpdateStockAsync(Guid productId, int quantity);
    Task<bool> DeleteProductAsync(Guid productId);
}
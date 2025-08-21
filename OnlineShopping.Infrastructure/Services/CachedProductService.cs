using AutoMapper;
using Microsoft.Extensions.Logging;
using OnlineShopping.Core.DTOs;
using OnlineShopping.Core.Interfaces;

namespace OnlineShopping.Infrastructure.Services;

public class CachedProductService : IProductService
{
    private readonly IProductService _productService;
    private readonly ICacheService _cacheService;
    private readonly ILogger<CachedProductService> _logger;
    private const string ProductCacheKeyPrefix = "product:";
    private const string ProductsListCacheKey = "products:all";
    private const string ActiveProductsListCacheKey = "products:active";

    public CachedProductService(
        ProductService productService,
        ICacheService cacheService,
        ILogger<CachedProductService> logger)
    {
        _productService = productService;
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task<ProductDto> CreateProductAsync(CreateProductDto dto)
    {
        var result = await _productService.CreateProductAsync(dto);
        
        await _cacheService.RemoveAsync(ProductsListCacheKey);
        await _cacheService.RemoveAsync(ActiveProductsListCacheKey);
        
        await _cacheService.SetAsync($"{ProductCacheKeyPrefix}{result.Id}", result, TimeSpan.FromMinutes(10));
        
        return result;
    }

    public async Task<ProductDto?> GetProductAsync(Guid productId)
    {
        var cacheKey = $"{ProductCacheKeyPrefix}{productId}";
        
        var cachedProduct = await _cacheService.GetAsync<ProductDto>(cacheKey);
        if (cachedProduct != null)
        {
            return cachedProduct;
        }

        var product = await _productService.GetProductAsync(productId);
        if (product != null)
        {
            await _cacheService.SetAsync(cacheKey, product, TimeSpan.FromMinutes(10));
        }

        return product;
    }

    public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
    {
        var cachedProducts = await _cacheService.GetAsync<IEnumerable<ProductDto>>(ProductsListCacheKey);
        if (cachedProducts != null)
        {
            return cachedProducts;
        }

        var products = await _productService.GetAllProductsAsync();
        
        await _cacheService.SetAsync(ProductsListCacheKey, products, TimeSpan.FromMinutes(5));
        
        return products;
    }

    public async Task<IEnumerable<ProductDto>> GetActiveProductsAsync()
    {
        var cachedProducts = await _cacheService.GetAsync<IEnumerable<ProductDto>>(ActiveProductsListCacheKey);
        if (cachedProducts != null)
        {
            return cachedProducts;
        }

        var products = await _productService.GetActiveProductsAsync();
        
        await _cacheService.SetAsync(ActiveProductsListCacheKey, products, TimeSpan.FromMinutes(5));
        
        return products;
    }

    public async Task<IEnumerable<ProductDto>> GetProductsByCategoryAsync(Guid categoryId)
    {
        var cacheKey = $"products:category:{categoryId}";
        
        var cachedProducts = await _cacheService.GetAsync<IEnumerable<ProductDto>>(cacheKey);
        if (cachedProducts != null)
        {
            return cachedProducts;
        }

        var products = await _productService.GetProductsByCategoryAsync(categoryId);
        
        await _cacheService.SetAsync(cacheKey, products, TimeSpan.FromMinutes(5));
        
        return products;
    }

    public async Task<IEnumerable<ProductDto>> SearchProductsAsync(string searchTerm)
    {
        return await _productService.SearchProductsAsync(searchTerm);
    }

    public async Task<ProductDto?> UpdateProductAsync(Guid productId, UpdateProductDto dto)
    {
        var result = await _productService.UpdateProductAsync(productId, dto);
        
        if (result != null)
        {
            await _cacheService.RemoveAsync($"{ProductCacheKeyPrefix}{productId}");
            await _cacheService.RemoveAsync(ProductsListCacheKey);
            await _cacheService.RemoveAsync(ActiveProductsListCacheKey);
            await _cacheService.RemoveByPatternAsync("products:category:*");
            
            await _cacheService.SetAsync($"{ProductCacheKeyPrefix}{productId}", result, TimeSpan.FromMinutes(10));
        }

        return result;
    }

    public async Task<bool> DeleteProductAsync(Guid productId)
    {
        var result = await _productService.DeleteProductAsync(productId);
        
        if (result)
        {
            await _cacheService.RemoveAsync($"{ProductCacheKeyPrefix}{productId}");
            await _cacheService.RemoveAsync(ProductsListCacheKey);
            await _cacheService.RemoveAsync(ActiveProductsListCacheKey);
            await _cacheService.RemoveByPatternAsync("products:category:*");
        }

        return result;
    }

    public async Task<bool> UpdateStockAsync(Guid productId, int quantity)
    {
        var result = await _productService.UpdateStockAsync(productId, quantity);
        
        if (result)
        {
            await _cacheService.RemoveAsync($"{ProductCacheKeyPrefix}{productId}");
        }

        return result;
    }

    public async Task<ProductDto?> GetProductBySkuAsync(string sku)
    {
        var cacheKey = $"product:sku:{sku}";
        
        var cachedProduct = await _cacheService.GetAsync<ProductDto>(cacheKey);
        if (cachedProduct != null)
        {
            return cachedProduct;
        }

        var product = await _productService.GetProductBySkuAsync(sku);
        if (product != null)
        {
            await _cacheService.SetAsync(cacheKey, product, TimeSpan.FromMinutes(10));
        }

        return product;
    }
}
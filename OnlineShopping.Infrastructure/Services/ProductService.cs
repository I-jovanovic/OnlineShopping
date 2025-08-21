using AutoMapper;
using Microsoft.Extensions.Logging;
using OnlineShopping.Core.DTOs;
using OnlineShopping.Core.Entities;
using OnlineShopping.Core.Exceptions;
using OnlineShopping.Core.Interfaces;

namespace OnlineShopping.Infrastructure.Services;

/// <summary>
/// Product service implementation
/// </summary>
public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<ProductService> _logger;

    public ProductService(
        IProductRepository productRepository,
        ICategoryRepository categoryRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<ProductService> logger)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ProductDto> CreateProductAsync(CreateProductDto dto)
    {
        if (await _productRepository.SkuExistsAsync(dto.SKU))
        {
            throw new BusinessRuleViolationException($"Product with SKU {dto.SKU} already exists");
        }

        if (dto.CategoryId.HasValue)
        {
            var category = await _categoryRepository.GetByIdAsync(dto.CategoryId.Value);
            if (category == null)
            {
                throw new NotFoundException($"Category with ID {dto.CategoryId} not found");
            }
        }

        var product = _mapper.Map<Product>(dto);
        product.Id = Guid.NewGuid();
        product.CreatedAt = DateTime.UtcNow;
        product.UpdatedAt = DateTime.UtcNow;

        await _productRepository.AddAsync(product);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Product created with ID: {ProductId}, SKU: {SKU}", product.Id, product.Sku);

        return _mapper.Map<ProductDto>(product);
    }

    public async Task<ProductDto?> GetProductAsync(Guid productId)
    {
        var product = await _productRepository.GetWithCategoryAsync(productId);
        return product != null ? _mapper.Map<ProductDto>(product) : null;
    }

    public async Task<ProductDto?> GetProductBySkuAsync(string sku)
    {
        var product = await _productRepository.GetBySkuAsync(sku);
        return product != null ? _mapper.Map<ProductDto>(product) : null;
    }

    public async Task<IEnumerable<ProductDto>> GetProductsByCategoryAsync(Guid categoryId)
    {
        var products = await _productRepository.GetByCategoryAsync(categoryId);
        return _mapper.Map<IEnumerable<ProductDto>>(products);
    }

    public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
    {
        var products = await _productRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<ProductDto>>(products);
    }

    public async Task<IEnumerable<ProductDto>> GetActiveProductsAsync()
    {
        var products = await _productRepository.GetActiveProductsAsync();
        return _mapper.Map<IEnumerable<ProductDto>>(products);
    }

    public async Task<IEnumerable<ProductDto>> SearchProductsAsync(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return Enumerable.Empty<ProductDto>();
        }

        var products = await _productRepository.SearchAsync(searchTerm);
        return _mapper.Map<IEnumerable<ProductDto>>(products);
    }

    public async Task<ProductDto?> UpdateProductAsync(Guid productId, UpdateProductDto dto)
    {
        var product = await _productRepository.GetByIdAsync(productId);
        if (product == null)
        {
            return null;
        }

        if (!string.IsNullOrEmpty(dto.Sku) && dto.Sku != product.Sku)
        {
            if (await _productRepository.SkuExistsAsync(dto.Sku))
            {
                throw new BusinessRuleViolationException($"SKU {dto.Sku} is already in use");
            }
            product.Sku = dto.Sku;
        }

        if (dto.CategoryId.HasValue && dto.CategoryId != product.CategoryId)
        {
            var category = await _categoryRepository.GetByIdAsync(dto.CategoryId.Value);
            if (category == null)
            {
                throw new NotFoundException($"Category with ID {dto.CategoryId} not found");
            }
            product.CategoryId = dto.CategoryId;
        }

        if (!string.IsNullOrEmpty(dto.Name))
            product.Name = dto.Name;
        
        if (!string.IsNullOrEmpty(dto.Description))
            product.Description = dto.Description;
        
        if (dto.Price.HasValue)
            product.Price = dto.Price.Value;
        
        if (dto.StockQuantity.HasValue)
            product.StockQuantity = dto.StockQuantity.Value;
        
        if (!string.IsNullOrEmpty(dto.ImageUrl))
            product.ImageUrl = dto.ImageUrl;
        
        if (dto.IsActive.HasValue)
            product.IsActive = dto.IsActive.Value;

        product.UpdatedAt = DateTime.UtcNow;

        await _productRepository.UpdateAsync(product);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Product updated with ID: {ProductId}", product.Id);

        return _mapper.Map<ProductDto>(product);
    }

    public async Task<bool> UpdateStockAsync(Guid productId, int quantity)
    {
        var product = await _productRepository.GetByIdAsync(productId);
        if (product == null)
        {
            return false;
        }

        product.StockQuantity = quantity;
        product.UpdatedAt = DateTime.UtcNow;

        await _productRepository.UpdateAsync(product);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Product stock updated for ID: {ProductId}, New quantity: {Quantity}", productId, quantity);

        return true;
    }

    public async Task<bool> DeleteProductAsync(Guid productId)
    {
        var product = await _productRepository.GetByIdAsync(productId);
        if (product == null)
        {
            return false;
        }

        await _productRepository.DeleteAsync(product);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Product deleted with ID: {ProductId}", productId);

        return true;
    }

}
using Microsoft.EntityFrameworkCore;
using OnlineShopping.Core.Entities;
using OnlineShopping.Core.Interfaces;

namespace OnlineShopping.Infrastructure.Persistence.Repositories;

/// <summary>
/// Product repository implementation
/// </summary>
public class ProductRepository : Repository<Product>, IProductRepository
{
    public ProductRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Product?> GetBySkuAsync(string sku)
    {
        return await _dbSet
            .FirstOrDefaultAsync(p => p.Sku.ToLower() == sku.ToLower());
    }

    public async Task<IEnumerable<Product>> GetByCategoryAsync(Guid categoryId)
    {
        return await _dbSet
            .Where(p => p.CategoryId == categoryId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetActiveProductsAsync()
    {
        return await _dbSet
            .Where(p => p.IsActive)
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> SearchAsync(string searchTerm)
    {
        var term = searchTerm.ToLower();
        return await _dbSet
            .Where(p => p.Name.ToLower().Contains(term) || 
                       (p.Description != null && p.Description.ToLower().Contains(term)) ||
                       p.Sku.ToLower().Contains(term))
            .ToListAsync();
    }

    public async Task<bool> SkuExistsAsync(string sku)
    {
        return await _dbSet
            .AnyAsync(p => p.Sku.ToLower() == sku.ToLower());
    }

    public async Task<Product?> GetWithCategoryAsync(Guid productId)
    {
        return await _dbSet
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == productId);
    }
}
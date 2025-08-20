using Microsoft.EntityFrameworkCore;
using OnlineShopping.Core.Entities;
using OnlineShopping.Core.Interfaces;

namespace OnlineShopping.Infrastructure.Persistence.Repositories;

/// <summary>
/// Shopping cart repository implementation
/// </summary>
public class ShoppingCartRepository : Repository<ShoppingCart>, IShoppingCartRepository
{
    public ShoppingCartRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<ShoppingCart?> GetByCustomerIdAsync(Guid customerId)
    {
        return await _dbSet
            .FirstOrDefaultAsync(c => c.CustomerId == customerId);
    }

    public async Task<ShoppingCart?> GetWithItemsAsync(Guid cartId)
    {
        return await _dbSet
            .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
            .FirstOrDefaultAsync(c => c.Id == cartId);
    }

    public async Task<ShoppingCart?> GetActiveCartByCustomerIdAsync(Guid customerId)
    {
        return await _dbSet
            .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
            .Where(c => c.CustomerId == customerId)
            .OrderByDescending(c => c.CreatedAt)
            .FirstOrDefaultAsync();
    }

    public async Task RemoveExpiredCartsAsync()
    {
        var expiredDate = DateTime.UtcNow.AddDays(-30);
        var expiredCarts = await _dbSet
            .Where(c => c.UpdatedAt < expiredDate)
            .ToListAsync();

        _dbSet.RemoveRange(expiredCarts);
    }
}
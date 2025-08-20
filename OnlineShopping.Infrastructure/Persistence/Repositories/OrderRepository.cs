using Microsoft.EntityFrameworkCore;
using OnlineShopping.Core.Entities;
using OnlineShopping.Core.Interfaces;

namespace OnlineShopping.Infrastructure.Persistence.Repositories;

/// <summary>
/// Order repository implementation
/// </summary>
public class OrderRepository : Repository<Order>, IOrderRepository
{
    public OrderRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Order?> GetByOrderNumberAsync(string orderNumber)
    {
        return await _dbSet
            .FirstOrDefaultAsync(o => o.OrderNumber == orderNumber);
    }

    public async Task<IEnumerable<Order>> GetByCustomerIdAsync(Guid customerId)
    {
        return await _dbSet
            .Where(o => o.CustomerId == customerId)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Order>> GetByStatusAsync(OrderStatus status)
    {
        return await _dbSet
            .Where(o => o.Status == status)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();
    }

    public async Task<Order?> GetWithItemsAsync(Guid orderId)
    {
        return await _dbSet
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .FirstOrDefaultAsync(o => o.Id == orderId);
    }

    public async Task<Order?> GetWithAllDetailsAsync(Guid orderId)
    {
        return await _dbSet
            .Include(o => o.Customer)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .Include(o => o.ShippingAddress)
            .Include(o => o.BillingAddress)
            .Include(o => o.Payment)
            .FirstOrDefaultAsync(o => o.Id == orderId);
    }

    public async Task<string> GenerateUniqueOrderNumberAsync()
    {
        string orderNumber;
        bool exists;
        
        do
        {
            var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            var random = new Random().Next(1000, 9999);
            orderNumber = $"ORD-{timestamp}-{random}";
            
            exists = await _dbSet.AnyAsync(o => o.OrderNumber == orderNumber);
        } while (exists);

        return orderNumber;
    }
}
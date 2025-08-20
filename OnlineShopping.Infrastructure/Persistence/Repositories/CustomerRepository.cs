using Microsoft.EntityFrameworkCore;
using OnlineShopping.Core.Entities;
using OnlineShopping.Core.Interfaces;

namespace OnlineShopping.Infrastructure.Persistence.Repositories;

/// <summary>
/// Customer repository implementation
/// </summary>
public class CustomerRepository : Repository<Customer>, ICustomerRepository
{
    public CustomerRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Customer?> GetByEmailAsync(string email)
    {
        return await _dbSet
            .FirstOrDefaultAsync(c => c.Email.ToLower() == email.ToLower());
    }

    public async Task<Customer?> GetWithAddressesAsync(Guid customerId)
    {
        return await _dbSet
            .Include(c => c.Addresses)
            .FirstOrDefaultAsync(c => c.Id == customerId);
    }

    public async Task<Customer?> GetWithOrdersAsync(Guid customerId)
    {
        return await _dbSet
            .Include(c => c.Orders)
            .FirstOrDefaultAsync(c => c.Id == customerId);
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _dbSet
            .AnyAsync(c => c.Email.ToLower() == email.ToLower());
    }
}
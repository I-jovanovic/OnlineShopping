using OnlineShopping.Core.Entities;

namespace OnlineShopping.Core.Interfaces;

/// <summary>
/// Customer repository interface
/// </summary>
public interface ICustomerRepository : IRepository<Customer>
{
    Task<Customer?> GetByEmailAsync(string email);
    Task<Customer?> GetWithAddressesAsync(Guid customerId);
    Task<Customer?> GetWithOrdersAsync(Guid customerId);
    Task<bool> EmailExistsAsync(string email);
}
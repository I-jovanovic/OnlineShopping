using OnlineShopping.Core.Entities;

namespace OnlineShopping.Core.Interfaces;

/// <summary>
/// Customer service interface
/// </summary>
public interface ICustomerService : IService
{
    Task<Customer> CreateCustomerAsync(string email, string firstName, string lastName, string? phoneNumber);
    Task<Customer?> GetCustomerByIdAsync(Guid id);
    Task<Customer?> GetCustomerByEmailAsync(string email);
    Task<IEnumerable<Customer>> GetAllCustomersAsync();
    Task<Customer> UpdateCustomerAsync(Customer customer);
    Task DeleteCustomerAsync(Guid id);
    Task<bool> CustomerExistsAsync(string email);
    
    Task<Address> AddAddressAsync(Guid customerId, Address address);
    Task<Address> UpdateAddressAsync(Address address);
    Task DeleteAddressAsync(Guid addressId);
    Task<IEnumerable<Address>> GetCustomerAddressesAsync(Guid customerId);
}
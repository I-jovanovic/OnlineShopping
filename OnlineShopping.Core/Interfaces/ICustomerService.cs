using OnlineShopping.Core.DTOs;

namespace OnlineShopping.Core.Interfaces;

/// <summary>
/// Customer service interface
/// </summary>
public interface ICustomerService : IService
{
    Task<CustomerDto> CreateCustomerAsync(CreateCustomerDto dto);
    Task<CustomerDto?> GetCustomerAsync(Guid customerId);
    Task<CustomerDto?> GetCustomerByEmailAsync(string email);
    Task<CustomerDto> UpdateCustomerAsync(Guid customerId, UpdateCustomerDto dto);
    Task<bool> DeleteCustomerAsync(Guid customerId);
    Task<IEnumerable<AddressDto>> GetCustomerAddressesAsync(Guid customerId);
}
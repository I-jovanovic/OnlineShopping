using AutoMapper;
using Microsoft.Extensions.Logging;
using OnlineShopping.Core.DTOs;
using OnlineShopping.Core.Entities;
using OnlineShopping.Core.Exceptions;
using OnlineShopping.Core.Interfaces;

namespace OnlineShopping.Infrastructure.Services;

/// <summary>
/// Customer service implementation
/// </summary>
public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<CustomerService> _logger;

    public CustomerService(
        ICustomerRepository customerRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<CustomerService> logger)
    {
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<CustomerDto> CreateCustomerAsync(CreateCustomerDto dto)
    {
        if (await _customerRepository.EmailExistsAsync(dto.Email))
        {
            throw new BusinessRuleViolationException($"Customer with email {dto.Email} already exists");
        }

        var customer = _mapper.Map<Customer>(dto);
        customer.Id = Guid.NewGuid();
        customer.CreatedAt = DateTime.UtcNow;
        customer.UpdatedAt = DateTime.UtcNow;

        await _customerRepository.AddAsync(customer);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Customer created with ID: {CustomerId}", customer.Id);

        return _mapper.Map<CustomerDto>(customer);
    }

    public async Task<CustomerDto?> GetCustomerAsync(Guid customerId)
    {
        var customer = await _customerRepository.GetByIdAsync(customerId);
        return customer != null ? _mapper.Map<CustomerDto>(customer) : null;
    }

    public async Task<CustomerDto?> GetCustomerByEmailAsync(string email)
    {
        var customer = await _customerRepository.GetByEmailAsync(email);
        return customer != null ? _mapper.Map<CustomerDto>(customer) : null;
    }

    public async Task<CustomerDto> UpdateCustomerAsync(Guid customerId, UpdateCustomerDto dto)
    {
        var customer = await _customerRepository.GetByIdAsync(customerId);
        if (customer == null)
        {
            throw new NotFoundException($"Customer with ID {customerId} not found");
        }

        if (!string.IsNullOrEmpty(dto.Email) && dto.Email != customer.Email)
        {
            if (await _customerRepository.EmailExistsAsync(dto.Email))
            {
                throw new BusinessRuleViolationException($"Email {dto.Email} is already in use");
            }
            customer.Email = dto.Email;
        }

        if (!string.IsNullOrEmpty(dto.FirstName))
            customer.FirstName = dto.FirstName;
        
        if (!string.IsNullOrEmpty(dto.LastName))
            customer.LastName = dto.LastName;
        
        if (!string.IsNullOrEmpty(dto.Phone))
            customer.Phone = dto.Phone;
        
        if (dto.DateOfBirth.HasValue)
            customer.DateOfBirth = dto.DateOfBirth.Value;

        customer.UpdatedAt = DateTime.UtcNow;

        await _customerRepository.UpdateAsync(customer);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Customer updated with ID: {CustomerId}", customer.Id);

        return _mapper.Map<CustomerDto>(customer);
    }

    public async Task<bool> DeleteCustomerAsync(Guid customerId)
    {
        var customer = await _customerRepository.GetWithOrdersAsync(customerId);
        if (customer == null)
        {
            return false;
        }

        if (customer.Orders?.Any() == true)
        {
            throw new BusinessRuleViolationException("Cannot delete customer with existing orders");
        }

        await _customerRepository.DeleteAsync(customer);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Customer deleted with ID: {CustomerId}", customerId);

        return true;
    }

    public async Task<IEnumerable<AddressDto>> GetCustomerAddressesAsync(Guid customerId)
    {
        var customer = await _customerRepository.GetWithAddressesAsync(customerId);
        if (customer == null)
        {
            throw new NotFoundException($"Customer with ID {customerId} not found");
        }

        return _mapper.Map<IEnumerable<AddressDto>>(customer.Addresses ?? Enumerable.Empty<Address>());
    }

}
using Microsoft.AspNetCore.Mvc;
using OnlineShopping.Core.DTOs;
using OnlineShopping.Core.Exceptions;
using OnlineShopping.Core.Interfaces;

namespace OnlineShopping.Controllers;

/// <summary>
/// Customer management endpoints
/// </summary>
public class CustomersController : ApiControllerBase
{
    private readonly ICustomerService _customerService;
    private readonly ILogger<CustomersController> _logger;

    public CustomersController(
        ICustomerService customerService,
        ILogger<CustomersController> logger)
    {
        _customerService = customerService;
        _logger = logger;
    }

    /// <summary>
    /// Create a new customer
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CustomerDto>> CreateCustomer([FromBody] CreateCustomerDto dto)
    {
        try
        {
            var customer = await _customerService.CreateCustomerAsync(dto);
            return CreatedAtAction(nameof(GetCustomer), new { id = customer.Id }, customer);
        }
        catch (BusinessRuleViolationException ex)
        {
            _logger.LogWarning(ex, "Business rule violation creating customer");
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Get customer by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CustomerDto>> GetCustomer(Guid id)
    {
        var customer = await _customerService.GetCustomerAsync(id);
        if (customer == null)
        {
            return NotFound();
        }
        return Ok(customer);
    }

    /// <summary>
    /// Get customer by email
    /// </summary>
    [HttpGet("by-email")]
    [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CustomerDto>> GetCustomerByEmail([FromQuery] string email)
    {
        var customer = await _customerService.GetCustomerByEmailAsync(email);
        if (customer == null)
        {
            return NotFound();
        }
        return Ok(customer);
    }

    /// <summary>
    /// Update customer
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CustomerDto>> UpdateCustomer(Guid id, [FromBody] UpdateCustomerDto dto)
    {
        try
        {
            var customer = await _customerService.UpdateCustomerAsync(id, dto);
            return Ok(customer);
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
        catch (BusinessRuleViolationException ex)
        {
            _logger.LogWarning(ex, "Business rule violation updating customer");
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Delete customer
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteCustomer(Guid id)
    {
        try
        {
            var deleted = await _customerService.DeleteCustomerAsync(id);
            if (!deleted)
            {
                return NotFound();
            }
            return NoContent();
        }
        catch (BusinessRuleViolationException ex)
        {
            _logger.LogWarning(ex, "Business rule violation deleting customer");
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Get customer addresses
    /// </summary>
    [HttpGet("{id:guid}/addresses")]
    [ProducesResponseType(typeof(IEnumerable<AddressDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<AddressDto>>> GetCustomerAddresses(Guid id)
    {
        try
        {
            var addresses = await _customerService.GetCustomerAddressesAsync(id);
            return Ok(addresses);
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
    }
}
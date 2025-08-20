namespace OnlineShopping.Core.DTOs;

/// <summary>
/// DTO for creating a customer
/// </summary>
public class CreateCustomerDto
{
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
}

/// <summary>
/// DTO for updating a customer
/// </summary>
public class UpdateCustomerDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
}
namespace OnlineShopping.Core.DTOs;

/// <summary>
/// DTO for creating a customer
/// </summary>
public class CreateCustomerDto
{
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public DateTime? DateOfBirth { get; set; }
}

/// <summary>
/// DTO for updating a customer
/// </summary>
public class UpdateCustomerDto
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public DateTime? DateOfBirth { get; set; }
}
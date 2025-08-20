namespace OnlineShopping.Core.Entities;

/// <summary>
/// Address entity
/// </summary>
public class Address : BaseEntity
{
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string? AddressLine2 { get; set; }
    public AddressType Type { get; set; }
    public bool IsDefault { get; set; }
    
    public Guid? CustomerId { get; set; }
    
    public virtual Customer? Customer { get; set; }
}

public enum AddressType
{
    Shipping = 0,
    Billing = 1,
    Both = 2
}
namespace OnlineShopping.Core.Entities;

/// <summary>
/// Customer entity
/// </summary>
public class Customer : BaseEntity
{
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public DateTime? DateOfBirth { get; set; }
    
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    public virtual ICollection<Address> Addresses { get; set; } = new List<Address>();
}
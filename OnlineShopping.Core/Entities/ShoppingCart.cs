namespace OnlineShopping.Core.Entities;

/// <summary>
/// Shopping cart entity
/// </summary>
public class ShoppingCart : BaseEntity
{
    public Guid CustomerId { get; set; }
    public DateTime? ExpiresAt { get; set; }
    
    public virtual Customer Customer { get; set; } = null!;
    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
}
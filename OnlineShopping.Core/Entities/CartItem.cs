namespace OnlineShopping.Core.Entities;

/// <summary>
/// Shopping cart item entity
/// </summary>
public class CartItem : BaseEntity
{
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    
    public Guid ShoppingCartId { get; set; }
    public Guid ProductId { get; set; }
    
    public virtual ShoppingCart ShoppingCart { get; set; } = null!;
    public virtual Product Product { get; set; } = null!;
}
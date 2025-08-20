namespace OnlineShopping.Core.Entities;

/// <summary>
/// Order item entity
/// </summary>
public class OrderItem : BaseEntity
{
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public decimal? Discount { get; set; }
    
    public Guid OrderId { get; set; }
    public Guid ProductId { get; set; }
    
    public virtual Order Order { get; set; } = null!;
    public virtual Product Product { get; set; } = null!;
}
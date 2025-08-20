namespace OnlineShopping.Core.Entities;

/// <summary>
/// Product entity
/// </summary>
public class Product : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    
    /// <summary>
    /// Stock Keeping Unit - unique merchant identifier for inventory tracking
    /// </summary>
    public string Sku { get; set; } = string.Empty;
    
    public int StockQuantity { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsActive { get; set; } = true;
    public decimal? Weight { get; set; }
    public string? Dimensions { get; set; }
    
    public Guid? CategoryId { get; set; }
    
    public virtual Category? Category { get; set; }
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
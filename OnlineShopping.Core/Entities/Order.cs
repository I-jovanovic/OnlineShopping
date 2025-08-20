namespace OnlineShopping.Core.Entities;

/// <summary>
/// Order entity
/// </summary>
public class Order : BaseEntity
{
    public string OrderNumber { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public OrderStatus Status { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal? TaxAmount { get; set; }
    public decimal? ShippingAmount { get; set; }
    public string? Notes { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public DateTime? ShippedDate { get; set; }
    public DateTime? DeliveredDate { get; set; }
    
    public Guid CustomerId { get; set; }
    public Guid ShippingAddressId { get; set; }
    public Guid BillingAddressId { get; set; }
    
    public virtual Customer Customer { get; set; } = null!;
    public virtual Address ShippingAddress { get; set; } = null!;
    public virtual Address BillingAddress { get; set; } = null!;
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    public virtual Payment? Payment { get; set; }
}

public enum OrderStatus
{
    Pending = 0,
    Processing = 1,
    Shipped = 2,
    Delivered = 3,
    Cancelled = 4,
    Refunded = 5
}
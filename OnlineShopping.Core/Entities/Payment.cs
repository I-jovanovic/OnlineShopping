namespace OnlineShopping.Core.Entities;

/// <summary>
/// Payment entity
/// </summary>
public class Payment : BaseEntity
{
    public string PaymentMethod { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public PaymentStatus Status { get; set; }
    public string? TransactionId { get; set; }
    public string? GatewayResponse { get; set; }
    public DateTime? ProcessedAt { get; set; }
    public DateTime PaymentDate { get; set; }
    
    public Guid OrderId { get; set; }
    
    public virtual Order Order { get; set; } = null!;
}

public enum PaymentStatus
{
    Pending = 0,
    Processing = 1,
    Completed = 2,
    Failed = 3,
    Refunded = 4,
    Cancelled = 5
}
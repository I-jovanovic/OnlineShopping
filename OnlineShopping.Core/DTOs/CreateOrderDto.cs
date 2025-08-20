namespace OnlineShopping.Core.DTOs;

public class CreateOrderDto
{
    public Guid CustomerId { get; set; }
    public Guid CartId { get; set; }
    public Guid ShippingAddressId { get; set; }
    public Guid BillingAddressId { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
}
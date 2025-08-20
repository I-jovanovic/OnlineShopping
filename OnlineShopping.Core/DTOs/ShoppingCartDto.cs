namespace OnlineShopping.Core.DTOs;

public class ShoppingCartDto
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public List<CartItemDto> Items { get; set; } = new();
    public decimal TotalAmount { get; set; }
    public int TotalItems { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CartItemDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductSKU { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public decimal Subtotal { get; set; }
}
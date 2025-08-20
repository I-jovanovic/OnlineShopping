namespace OnlineShopping.Core.DTOs;

/// <summary>
/// DTO for creating a product
/// </summary>
public class CreateProductDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public string Sku { get; set; } = string.Empty;
    public int StockQuantity { get; set; }
    public Guid CategoryId { get; set; }
    public string? ImageUrl { get; set; }
    public decimal? Weight { get; set; }
    public string? Dimensions { get; set; }
}

/// <summary>
/// DTO for updating a product
/// </summary>
public class UpdateProductDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public string? ImageUrl { get; set; }
    public decimal? Weight { get; set; }
    public string? Dimensions { get; set; }
    public bool IsActive { get; set; } = true;
}
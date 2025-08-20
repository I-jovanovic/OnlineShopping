namespace OnlineShopping.Core.DTOs;

/// <summary>
/// DTO for product information
/// </summary>
public class ProductDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string SKU { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public Guid? CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// DTO for creating a product
/// </summary>
public class CreateProductDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string SKU { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public Guid? CategoryId { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsActive { get; set; } = true;
}

/// <summary>
/// DTO for updating a product
/// </summary>
public class UpdateProductDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Sku { get; set; }
    public decimal? Price { get; set; }
    public int? StockQuantity { get; set; }
    public Guid? CategoryId { get; set; }
    public string? ImageUrl { get; set; }
    public bool? IsActive { get; set; }
}
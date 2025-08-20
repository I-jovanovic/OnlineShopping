namespace OnlineShopping.Core.Entities;

/// <summary>
/// Product category entity
/// </summary>
public class Category : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsActive { get; set; } = true;
    
    public Guid? ParentCategoryId { get; set; }
    public virtual Category? ParentCategory { get; set; }
    
    public virtual ICollection<Category> SubCategories { get; set; } = new List<Category>();
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
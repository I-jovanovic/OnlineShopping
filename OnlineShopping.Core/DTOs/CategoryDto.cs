namespace OnlineShopping.Core.DTOs;

public class CategoryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid? ParentCategoryId { get; set; }
    public string? ParentCategoryName { get; set; }
    public int ProductCount { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateCategoryDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid? ParentCategoryId { get; set; }
}

public class UpdateCategoryDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public Guid? ParentCategoryId { get; set; }
}
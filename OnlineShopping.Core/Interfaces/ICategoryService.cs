using OnlineShopping.Core.Entities;

namespace OnlineShopping.Core.Interfaces;

/// <summary>
/// Category service interface
/// </summary>
public interface ICategoryService : IService
{
    Task<Category> CreateCategoryAsync(string name, string? description, Guid? parentCategoryId = null);
    Task<Category?> GetCategoryByIdAsync(Guid id);
    Task<IEnumerable<Category>> GetAllCategoriesAsync();
    Task<IEnumerable<Category>> GetRootCategoriesAsync();
    Task<IEnumerable<Category>> GetSubCategoriesAsync(Guid parentCategoryId);
    Task<Category> UpdateCategoryAsync(Category category);
    Task DeleteCategoryAsync(Guid id);
    Task<bool> HasProductsAsync(Guid categoryId);
    Task<bool> HasSubCategoriesAsync(Guid categoryId);
}
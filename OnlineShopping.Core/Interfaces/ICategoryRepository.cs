using OnlineShopping.Core.Entities;

namespace OnlineShopping.Core.Interfaces;

/// <summary>
/// Category repository interface
/// </summary>
public interface ICategoryRepository : IRepository<Category>
{
    Task<Category?> GetByNameAsync(string name);
    Task<IEnumerable<Category>> GetRootCategoriesAsync();
    Task<IEnumerable<Category>> GetSubCategoriesAsync(Guid parentId);
    Task<Category?> GetWithProductsAsync(Guid categoryId);
    Task<bool> HasProductsAsync(Guid categoryId);
    Task<bool> NameExistsAsync(string name, Guid? excludeId = null);
}
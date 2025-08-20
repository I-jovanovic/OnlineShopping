using OnlineShopping.Core.DTOs;

namespace OnlineShopping.Core.Interfaces;

/// <summary>
/// Category service interface
/// </summary>
public interface ICategoryService : IService
{
    Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto dto);
    Task<CategoryDto?> GetCategoryAsync(Guid categoryId);
    Task<IEnumerable<CategoryDto>> GetRootCategoriesAsync();
    Task<IEnumerable<CategoryDto>> GetSubCategoriesAsync(Guid parentId);
    Task<CategoryDto> UpdateCategoryAsync(Guid categoryId, UpdateCategoryDto dto);
    Task<bool> DeleteCategoryAsync(Guid categoryId);
}
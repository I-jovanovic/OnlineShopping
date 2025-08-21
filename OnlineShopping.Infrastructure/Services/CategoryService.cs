using AutoMapper;
using Microsoft.Extensions.Logging;
using OnlineShopping.Core.DTOs;
using OnlineShopping.Core.Entities;
using OnlineShopping.Core.Exceptions;
using OnlineShopping.Core.Interfaces;

namespace OnlineShopping.Infrastructure.Services;

/// <summary>
/// Category service implementation
/// </summary>
public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<CategoryService> _logger;

    public CategoryService(
        ICategoryRepository categoryRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<CategoryService> logger)
    {
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto dto)
    {
        if (await _categoryRepository.GetByNameAsync(dto.Name) != null)
        {
            throw new BusinessRuleViolationException($"Category with name {dto.Name} already exists");
        }

        if (dto.ParentCategoryId.HasValue)
        {
            var parentCategory = await _categoryRepository.GetByIdAsync(dto.ParentCategoryId.Value);
            if (parentCategory == null)
            {
                throw new NotFoundException($"Parent category with ID {dto.ParentCategoryId} not found");
            }
        }

        var category = _mapper.Map<Category>(dto);
        category.Id = Guid.NewGuid();
        category.CreatedAt = DateTime.UtcNow;
        category.UpdatedAt = DateTime.UtcNow;

        await _categoryRepository.AddAsync(category);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Category created with ID: {CategoryId}", category.Id);

        var createdCategory = await _categoryRepository.GetByIdAsync(category.Id);
        return _mapper.Map<CategoryDto>(createdCategory);
    }

    public async Task<CategoryDto?> GetCategoryAsync(Guid categoryId)
    {
        var category = await _categoryRepository.GetWithProductsAsync(categoryId);
        return category != null ? _mapper.Map<CategoryDto>(category) : null;
    }

    public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
    {
        var categories = await _categoryRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<CategoryDto>>(categories);
    }

    public async Task<IEnumerable<CategoryDto>> GetRootCategoriesAsync()
    {
        var categories = await _categoryRepository.GetRootCategoriesAsync();
        return _mapper.Map<IEnumerable<CategoryDto>>(categories);
    }

    public async Task<IEnumerable<CategoryDto>> GetSubCategoriesAsync(Guid parentId)
    {
        var categories = await _categoryRepository.GetSubCategoriesAsync(parentId);
        return _mapper.Map<IEnumerable<CategoryDto>>(categories);
    }

    public async Task<CategoryDto?> UpdateCategoryAsync(Guid categoryId, UpdateCategoryDto dto)
    {
        var category = await _categoryRepository.GetByIdAsync(categoryId);
        if (category == null)
        {
            return null;
        }

        if (!string.IsNullOrEmpty(dto.Name) && dto.Name != category.Name)
        {
            var existingCategory = await _categoryRepository.GetByNameAsync(dto.Name);
            if (existingCategory != null && existingCategory.Id != categoryId)
            {
                throw new BusinessRuleViolationException($"Category with name {dto.Name} already exists");
            }
        }

        if (dto.ParentCategoryId.HasValue && dto.ParentCategoryId != category.ParentCategoryId)
        {
            if (dto.ParentCategoryId == categoryId)
            {
                throw new BusinessRuleViolationException("Category cannot be its own parent");
            }

            var parentCategory = await _categoryRepository.GetByIdAsync(dto.ParentCategoryId.Value);
            if (parentCategory == null)
            {
                throw new NotFoundException($"Parent category with ID {dto.ParentCategoryId} not found");
            }
        }

        _mapper.Map(dto, category);
        category.UpdatedAt = DateTime.UtcNow;

        await _categoryRepository.UpdateAsync(category);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Category updated with ID: {CategoryId}", category.Id);

        return _mapper.Map<CategoryDto>(category);
    }

    public async Task<bool> DeleteCategoryAsync(Guid categoryId)
    {
        var category = await _categoryRepository.GetByIdAsync(categoryId);
        if (category == null)
        {
            return false;
        }

        if (await _categoryRepository.HasProductsAsync(categoryId))
        {
            throw new BusinessRuleViolationException("Cannot delete category with existing products");
        }

        var subCategories = await _categoryRepository.GetSubCategoriesAsync(categoryId);
        if (subCategories.Any())
        {
            throw new BusinessRuleViolationException("Cannot delete category with subcategories");
        }

        await _categoryRepository.DeleteAsync(category);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Category deleted with ID: {CategoryId}", categoryId);

        return true;
    }
}
using AutoMapper;
using OnlineShopping.Core.DTOs;
using OnlineShopping.Core.Entities;

namespace OnlineShopping.Infrastructure.Mappings;

public class CategoryMappingProfile : Profile
{
    public CategoryMappingProfile()
    {
        // Entity to DTO
        CreateMap<Category, CategoryDto>()
            .ForMember(dest => dest.ParentCategoryName, opt => opt.MapFrom(src => 
                src.ParentCategory != null ? src.ParentCategory.Name : null))
            .ForMember(dest => dest.ProductCount, opt => opt.MapFrom(src => 
                src.Products != null ? src.Products.Count : 0));
        
        // DTO to Entity
        CreateMap<CreateCategoryDto, Category>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.ParentCategory, opt => opt.Ignore())
            .ForMember(dest => dest.SubCategories, opt => opt.Ignore())
            .ForMember(dest => dest.Products, opt => opt.Ignore());
            
        CreateMap<UpdateCategoryDto, Category>()
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
    }
}
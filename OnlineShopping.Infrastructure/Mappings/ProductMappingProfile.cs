using AutoMapper;
using OnlineShopping.Core.DTOs;
using OnlineShopping.Core.Entities;

namespace OnlineShopping.Infrastructure.Mappings;

public class ProductMappingProfile : Profile
{
    public ProductMappingProfile()
    {
        // Entity to DTO
        CreateMap<Product, ProductDto>()
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : null));
        
        // DTO to Entity
        CreateMap<CreateProductDto, Product>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Category, opt => opt.Ignore())
            .ForMember(dest => dest.OrderItems, opt => opt.Ignore())
            .ForMember(dest => dest.Weight, opt => opt.Ignore())
            .ForMember(dest => dest.Dimensions, opt => opt.Ignore());
            
        CreateMap<UpdateProductDto, Product>()
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
    }
}
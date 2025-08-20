using AutoMapper;
using OnlineShopping.Core.DTOs;
using OnlineShopping.Core.Entities;

namespace OnlineShopping.Infrastructure.Mappings;

public class ShoppingCartMappingProfile : Profile
{
    public ShoppingCartMappingProfile()
    {
        // Entity to DTO
        CreateMap<ShoppingCart, ShoppingCartDto>()
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.CartItems))
            .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src => 
                src.CartItems != null ? src.CartItems.Sum(ci => ci.Quantity * ci.Price) : 0))
            .ForMember(dest => dest.TotalItems, opt => opt.MapFrom(src => 
                src.CartItems != null ? src.CartItems.Sum(ci => ci.Quantity) : 0))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt ?? DateTime.UtcNow));
            
        CreateMap<CartItem, CartItemDto>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => 
                src.Product != null ? src.Product.Name : string.Empty))
            .ForMember(dest => dest.ProductSKU, opt => opt.MapFrom(src => 
                src.Product != null ? src.Product.Sku : string.Empty))
            .ForMember(dest => dest.Subtotal, opt => opt.MapFrom(src => src.Quantity * src.Price));
            
        // DTO to Entity
        CreateMap<AddToCartDto, CartItem>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.ShoppingCartId, opt => opt.Ignore())
            .ForMember(dest => dest.Price, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.ShoppingCart, opt => opt.Ignore())
            .ForMember(dest => dest.Product, opt => opt.Ignore());
    }
}
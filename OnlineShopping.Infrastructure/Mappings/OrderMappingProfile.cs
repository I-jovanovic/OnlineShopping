using AutoMapper;
using OnlineShopping.Core.DTOs;
using OnlineShopping.Core.Entities;

namespace OnlineShopping.Infrastructure.Mappings;

public class OrderMappingProfile : Profile
{
    public OrderMappingProfile()
    {
        // Entity to DTO
        CreateMap<Order, OrderDto>()
            .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => 
                src.Customer != null ? $"{src.Customer.FirstName} {src.Customer.LastName}" : string.Empty))
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.OrderItems));
            
        CreateMap<OrderItem, OrderItemDto>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => 
                src.Product != null ? src.Product.Name : string.Empty))
            .ForMember(dest => dest.ProductSKU, opt => opt.MapFrom(src => 
                src.Product != null ? src.Product.Sku : string.Empty))
            .ForMember(dest => dest.Subtotal, opt => opt.MapFrom(src => src.Quantity * src.UnitPrice));
            
        // DTO to Entity
        CreateMap<CreateOrderDto, Order>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.OrderNumber, opt => opt.Ignore())
            .ForMember(dest => dest.OrderDate, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.Ignore())
            .ForMember(dest => dest.TotalAmount, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Customer, opt => opt.Ignore())
            .ForMember(dest => dest.ShippingAddress, opt => opt.Ignore())
            .ForMember(dest => dest.BillingAddress, opt => opt.Ignore())
            .ForMember(dest => dest.OrderItems, opt => opt.Ignore())
            .ForMember(dest => dest.Payment, opt => opt.Ignore())
            .ForMember(dest => dest.TaxAmount, opt => opt.Ignore())
            .ForMember(dest => dest.ShippingAmount, opt => opt.Ignore())
            .ForMember(dest => dest.Notes, opt => opt.Ignore())
            .ForMember(dest => dest.ShippedDate, opt => opt.Ignore())
            .ForMember(dest => dest.DeliveredDate, opt => opt.Ignore());
    }
}
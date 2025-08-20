using AutoMapper;
using OnlineShopping.Core.DTOs;
using OnlineShopping.Core.Entities;

namespace OnlineShopping.Infrastructure.Mappings;

public class CustomerMappingProfile : Profile
{
    public CustomerMappingProfile()
    {
        // Entity to DTO
        CreateMap<Customer, CustomerDto>();
        
        // DTO to Entity
        CreateMap<CreateCustomerDto, Customer>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Orders, opt => opt.Ignore())
            .ForMember(dest => dest.Addresses, opt => opt.Ignore());
            
        CreateMap<UpdateCustomerDto, Customer>()
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
            
        // Address mappings
        CreateMap<Address, AddressDto>();
        CreateMap<AddressDto, Address>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Customer, opt => opt.Ignore());
    }
}
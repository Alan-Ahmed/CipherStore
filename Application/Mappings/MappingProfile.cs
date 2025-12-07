using AutoMapper;
using Application.DTOs;
using Domain.Entities;

namespace Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Mappar från Product (Databas) -> ProductDto (Utåt)
        CreateMap<Product, ProductDto>().ReverseMap();

        // Mappar från Order -> OrderDto
        CreateMap<Order, OrderDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

        // Mappar varukorgsrad -> OrderItem (Vid skapande av order)
        CreateMap<CartItemDto, OrderItem>();
    }
}
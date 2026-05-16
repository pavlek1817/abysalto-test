using AutoMapper;
using CartService.Application.Contracts.GetCart.Models;
using CartService.Domain.Entities;

namespace CartService.Application.Mappings;

public class CartMappingProfile : Profile
{
    public CartMappingProfile()
    {
        CreateMap<CartItem, CartItemModel>()
            .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId.ToString()))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.UnitPrice));

        CreateMap<Cart, CartModel>();
    }
}

using AutoMapper;
using BaseTemplate.Application.Products.Dtos;
using BaseTemplate.Domain.Entities;

namespace BaseTemplate.Application.Products;

public sealed class ProductMappingProfile : Profile
{
    public ProductMappingProfile()
    {
        CreateMap<Product, ProductDto>()
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price.Amount))
            .ForMember(dest => dest.Currency, opt => opt.MapFrom(src => src.Price.Currency));
    }
}

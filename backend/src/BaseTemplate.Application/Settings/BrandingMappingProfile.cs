using AutoMapper;
using BaseTemplate.Application.Settings.Dtos;
using BaseTemplate.Domain.Entities;

namespace BaseTemplate.Application.Settings;

public sealed class BrandingMappingProfile : Profile
{
    public BrandingMappingProfile()
    {
        CreateMap<BrandingSettings, BrandingSettingsDto>();
    }
}

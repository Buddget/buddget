using AutoMapper;
using Buddget.BLL.DTOs;
using Buddget.DAL.Entities;

namespace Buddget.BLL.Mappers
{
    public class CategoryProfile : Profile
    {
        public CategoryProfile()
        {
            CreateMap<CategoryEntity, CategoryDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.IsDefault, opt => opt.MapFrom(src => src.IsDefault));
        }
    }
}

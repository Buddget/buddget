using AutoMapper;
using Buddget.BLL.DTOs;
using Buddget.DAL.Entities;

namespace Buddget.BLL.Mappers
{
    public class FinancialSpaceProfile : Profile
    {
        public FinancialSpaceProfile()
        {
            CreateMap<FinancialSpaceEntity, FinancialSpaceDto>()
                .ForMember(dest => dest.OwnerName, opt => opt.MapFrom(src => src.Owner.FirstName + " " + src.Owner.LastName));
        }
    }
}

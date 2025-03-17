using AutoMapper;
using Buddget.BLL.DTOs;
using Buddget.DAL.Entities;

namespace Buddget.BLL.Mappers
{
    public class FinancialGoalSpaceProfile : Profile
    {
        public FinancialGoalSpaceProfile()
        {
            CreateMap<FinancialGoalSpaceEntity, FinancialGoalDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.FinancialGoal.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.FinancialGoal.Name))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.FinancialGoal.Type))
                .ForMember(dest => dest.TargetAmount, opt => opt.MapFrom(src => src.FinancialGoal.TargetAmount))
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.FinancialGoal.StartDate))
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.FinancialGoal.EndDate));
        }
    }
}

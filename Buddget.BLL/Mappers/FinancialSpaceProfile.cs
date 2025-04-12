using AutoMapper;
using Buddget.BLL.DTOs;
using Buddget.Domain.Entities;

namespace Buddget.BLL.Mappers
{
    public class FinancialSpaceProfile : Profile
    {
        public FinancialSpaceProfile()
        {
            CreateMap<FinancialSpaceEntity, FinancialSpaceDto>()
                .ForMember(dest => dest.Goals, opt => opt.MapFrom(src => src.FinancialGoalSpaces.Select(fgs => fgs.FinancialGoal).ToList()))
                .ForMember(dest => dest.Members, opt => opt.MapFrom(src => src.Members))
                .ForMember(dest => dest.RecentTransactions, opt => opt.MapFrom(src => src.Transactions))
                .ForMember(dest => dest.OwnerId, opt => opt.MapFrom(src => src.OwnerId));
            CreateMap<FinancialGoalSpaceEntity, FinancialGoalDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.FinancialGoal.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.FinancialGoal.Name))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.FinancialGoal.Type))
                .ForMember(dest => dest.TargetAmount, opt => opt.MapFrom(src => src.FinancialGoal.TargetAmount))
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.FinancialGoal.StartDate))
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.FinancialGoal.EndDate));
            CreateMap<FinancialGoalEntity, FinancialGoalDto>();
            CreateMap<FinancialSpaceMemberEntity, FinancialSpaceMemberDto>();
            CreateMap<TransactionEntity, TransactionDto>();

            CreateMap<FinancialSpaceDto, FinancialSpaceEntity>()
                .ForMember(dest => dest.Owner, opt => opt.Ignore())
                .ForMember(dest => dest.FinancialGoalSpaces, opt => opt.Ignore())
                .ForMember(dest => dest.Members, opt => opt.Ignore())
                .ForMember(dest => dest.Transactions, opt => opt.Ignore());
        }
    }
}

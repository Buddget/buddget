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
                .ForMember(dest => dest.OwnerName, opt => opt.MapFrom(src => src.Owner.FirstName))
                .ForMember(dest => dest.OwnerLastName, opt => opt.MapFrom(src => src.Owner.LastName))
                .ForMember(dest => dest.Goals, opt => opt.MapFrom(src => src.FinancialGoalSpaces.Select(fgs => fgs.FinancialGoal)))
                .ForMember(dest => dest.Members, opt => opt.MapFrom(src => src.Members.Select(m => m.User))) // Точно вказано мапінг
                .ForMember(dest => dest.RecentTransactions, opt => opt.MapFrom(src => src.Transactions.OrderByDescending(t => t.Date).Take(6))); // Логіка обмеження транзакцій

            CreateMap<FinancialGoalEntity, FinancialGoalDto>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.TargetAmount, opt => opt.MapFrom(src => src.TargetAmount))
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate))
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate));

            CreateMap<UserEntity, UserDto>()
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.LastType, opt => opt.MapFrom(src => src.LastName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role))
                .ForMember(dest => dest.RegisteredAt, opt => opt.MapFrom(src => src.RegisteredAt));

            CreateMap<TransactionEntity, TransactionDto>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount))
                .ForMember(dest => dest.Currency, opt => opt.MapFrom(src => src.Currency))
                .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.Date))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name));
        }
    }
}

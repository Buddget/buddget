using AutoMapper;
using Buddget.BLL.DTOs;
using Buddget.Domain.Entities;

public class TransactionProfile : Profile
{
    public TransactionProfile()
    {
        CreateMap<TransactionEntity, TransactionDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount))
            .ForMember(dest => dest.Currency, opt => opt.MapFrom(src => src.Currency))
            .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.Date))
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : "N/A"))
            .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.Category != null ? src.Category.Id : (int?)null))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
            .ForMember(dest => dest.AuthorName, opt => opt.MapFrom(src => src.User.FirstName + " " + src.User.LastName))
            .ForMember(dest => dest.AuthorId, opt => opt.MapFrom(src => src.User.Id))
            .ForMember(dest => dest.FinancialSpaceId, opt => opt.MapFrom(src => src.FinancialSpaceId));

        CreateMap<TransactionDto, TransactionEntity>()
        .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
        .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount))
        .ForMember(dest => dest.Currency, opt => opt.MapFrom(src => src.Currency))
        .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.Date))
        .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.CategoryId))
        .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
        .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
        .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.AuthorId))
        .ForMember(dest => dest.FinancialSpaceId, opt => opt.MapFrom(src => src.FinancialSpaceId));
    }
}


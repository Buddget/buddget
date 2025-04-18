﻿using AutoMapper;
using Buddget.BLL.DTOs;
using Buddget.Domain.Entities;

public class FinancialSpaceMemberProfile : Profile
{
    public FinancialSpaceMemberProfile()
    {
        CreateMap<FinancialSpaceMemberEntity, FinancialSpaceMemberDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.User.Id))
            .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.User.FirstName))
            .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.User.LastName))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
            .ForMember(dest => dest.RegisteredAt, opt => opt.MapFrom(src => src.User.RegisteredAt))
            .ForMember(dest => dest.FinancialSpaceId, opt => opt.MapFrom(src => src.FinancialSpaceId))
            .ForMember(dest => dest.MemberRole, opt => opt.MapFrom(src => src.Role))
            .ForMember(dest => dest.Categories, opt => opt.MapFrom(src => src.User.Categories));

        CreateMap<CreateFinancialSpaceMemberDto, FinancialSpaceMemberEntity>()
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.FinancialSpaceId, opt => opt.MapFrom(src => src.FinancialSpaceId))
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role));

        CreateMap<FinancialSpaceMemberEntity, CreateFinancialSpaceMemberDto>()
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.FinancialSpaceId, opt => opt.MapFrom(src => src.FinancialSpaceId))
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role));
    }
}

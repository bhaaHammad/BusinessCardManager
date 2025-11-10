using AutoMapper;
using BusinessCardManager.Application.DTOs.BusinessCards;
using BusinessCardManager.Domain.Entities;

namespace BusinessCardManager.Application.Mapping
{
    public class BusinessCardProfile : Profile
    {
        public BusinessCardProfile()
        {
            CreateMap<BusinessCardRequestDto, BusinessCard>();

            CreateMap<BusinessCard, BusinessCardResponseDto>();
        }
    }
}

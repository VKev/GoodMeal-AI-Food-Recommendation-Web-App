using AutoMapper;
using Domain.Entities;
using Application.Business.Queries.GetAllBusinessesQuery;
using Application.Business.Queries.GetBusinessByIdQuery;
using Application.Business.Queries.GetMyBusinessQuery;
using Application.Business.Queries.GetBusinessRestaurantsQuery;
using Application.Business.Commands.CreateBusinessCommand;
using Application.Business.Commands.UpdateBusinessCommand;
using SharedLibrary.Contracts.Business;
using DisableBusinessResponse = Application.Business.Commands.DisableBusinessCommand.DisableBusinessResponse;
using EnableBusinessResponse = Application.Business.Commands.EnableBusinessCommand.EnableBusinessResponse;

namespace Application.Common.Mapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateBusinessMappings();
            CreateBusinessRestaurantMappings();
        }

        private void CreateBusinessMappings()
        {
            CreateMap<Domain.Entities.Business, BusinessInfo>()
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive ?? true));

            CreateMap<Domain.Entities.Business, GetBusinessByIdResponse>()
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive ?? true));

            CreateMap<Domain.Entities.Business, GetMyBusinessResponse>()
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive ?? true));

            CreateMap<Domain.Entities.Business, CreateBusinessResponse>()
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive ?? true));

            CreateMap<Domain.Entities.Business, UpdateBusinessResponse>()
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive ?? true));

            CreateMap<Domain.Entities.Business, DisableBusinessResponse>()
                .ForMember(dest => dest.IsDisable, opt => opt.MapFrom(src => src.IsDisable ?? false));

            CreateMap<Domain.Entities.Business, EnableBusinessResponse>()
                .ForMember(dest => dest.IsDisable, opt => opt.MapFrom(src => src.IsDisable ?? false));
            
            CreateMap<Domain.Entities.Business, BusinessDto>().ReverseMap();
            CreateMap<BusinessInfo, BusinessDto>().ReverseMap();
        }

        private void CreateBusinessRestaurantMappings()
        {
            CreateMap<BusinessRestaurant, BusinessRestaurantInfo>()
                .ForMember(dest => dest.Restaurant, opt => opt.Ignore());
        }
    }
}
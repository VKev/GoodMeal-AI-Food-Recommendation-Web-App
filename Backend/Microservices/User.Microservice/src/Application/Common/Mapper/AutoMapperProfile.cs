using Application.Users.Commands;
using Application.Users.Queries;
using AutoMapper;
using Domain.Entities;

namespace Application.Common.Mapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<CreateUserCommand, User>()
                .ForMember(dest => dest.IdentityId, opt => opt.MapFrom(src => (string?)null))
                .ReverseMap();
            CreateMap<User, GetUserResponse>().ReverseMap();
        }
    }
}
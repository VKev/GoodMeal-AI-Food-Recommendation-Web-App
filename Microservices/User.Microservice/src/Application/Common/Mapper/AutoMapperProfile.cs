using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            CreateMap<CreateUserCommand, User>();
            CreateMap<User, CreateUserCommand>();

            CreateMap<GetUserResponse, User>();
            CreateMap<User, GetUserResponse>();
        }
        
    }
}
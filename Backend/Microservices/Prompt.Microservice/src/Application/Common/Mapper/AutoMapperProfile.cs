using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Prompt.Commands;
using Application.Prompt.Queries;
using AutoMapper;
using Infrastructure;

namespace Application.Common.Mapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<CreatePromptSessionCommand, PromptSession>();
            CreateMap<PromptSession, CreatePromptSessionCommand>();

            CreateMap<GetPromptSessionResponse, PromptSession>();
            CreateMap<PromptSession, GetPromptSessionResponse>();

            CreateMap<CreateMessageCommand, Message>();
            CreateMap<Message, CreateMessageCommand>();

            CreateMap<GetMessageResponse, Message>();
            CreateMap<Message, GetMessageResponse>();

            CreateMap<UpdateMessageCommand, Message>();
            CreateMap<Message, UpdateMessageCommand>();
        }
    }
}
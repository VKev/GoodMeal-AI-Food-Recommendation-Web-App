using AutoMapper;
using Domain.Entities;
using Application.Subscriptions.Queries.GetAllSubscriptionsQuery;
using Application.Subscriptions.Queries.GetSubscriptionByIdQuery;
using Application.Subscriptions.Commands.CreateSubscriptionCommand;
using Application.Subscriptions.Commands.UpdateSubscriptionCommand;
using Application.Subscriptions.Commands.DeleteSubscriptionCommand;

namespace Application.Common.Mapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateSubscriptionMappings();
            CreateUserSubscriptionMappings();
        }

        private void CreateSubscriptionMappings()
        {
            CreateMap<Subscription, SubscriptionInfo>()
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive));

            CreateMap<Subscription, GetSubscriptionByIdResponse>()
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive));

            CreateMap<Subscription, CreateSubscriptionResponse>()
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive));

            CreateMap<Subscription, UpdateSubscriptionResponse>()
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive));

            CreateMap<Subscription, DeleteSubscriptionResponse>()
                .ForMember(dest => dest.IsDisable, opt => opt.MapFrom(src => src.IsDisable ?? false));
        }

        private void CreateUserSubscriptionMappings()
        {
            CreateMap<UserSubscription, Application.UserSubscriptions.Commands.SubscribeUserCommand.SubscribeUserResponse>()
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive));
        }
    }
}
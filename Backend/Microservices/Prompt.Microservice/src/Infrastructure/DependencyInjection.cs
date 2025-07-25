using System;
using Domain.Repositories;
using SharedLibrary.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Infrastructure.Configs;
using Infrastructure.Repositories;
using MassTransit;
using Microsoft.Extensions.Configuration;
using SharedLibrary.Common.Event;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddScoped<IPromptSessionRepository, PromptSessionRepository>();
            services.AddScoped<IMessageRepository, MessageRepository>();
            services.AddScoped<IMessageRestaurantRepository, MessageRestaurantRepository>();
            services.AddScoped<IFoodImageRepository, FoodImageRepository>();
            services.AddSingleton<EnvironmentConfig>();
            services.AddScoped<EventBuffer>();
            services.AddScoped<IEventBuffer>(sp => sp.GetRequiredService<EventBuffer>());
            services.AddScoped<IEventUnitOfWork>(sp => sp.GetRequiredService<EventBuffer>());
            services.AddScoped<IEventFlusher>(sp => sp.GetRequiredService<EventBuffer>());

            

            return services;
        }
    }
}
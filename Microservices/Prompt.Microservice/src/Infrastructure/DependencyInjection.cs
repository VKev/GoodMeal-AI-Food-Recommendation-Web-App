using System;
using Domain.Repositories;
using SharedLibrary.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Infrastructure.Configs;
using Infrastructure.Repositories;
using MassTransit;
using Microsoft.Extensions.Configuration;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddScoped<IPromptSessionRepository, PromptSessionRepository>();
            services.AddScoped<IMessageRepository, MessageRepository>();
            services.AddScoped<IMessageRestaurantRepository, MessageRestaurantRepository>();
            var envConfig = new EnvironmentConfig();
            configuration.Bind("EnvironmentConfig", envConfig); // or load from env manually if you use .env

            services.AddSingleton(envConfig);
            services.AddMassTransit(busConfigurator =>
            {
                busConfigurator.SetKebabCaseEndpointNameFormatter();
                busConfigurator.UsingRabbitMq((context, configurator) =>
                {
                    var config = context.GetRequiredService<EnvironmentConfig>();

                    configurator.Host(new Uri($"rabbitmq://{config.RabbitMqHost}:{config.RabbitMqPort}/"), h =>
                    {
                        h.Username(config.RabbitMqUser);
                        h.Password(config.RabbitMqPassword);
                    });
                    configurator.ConfigureEndpoints(context);
                });
            });

            return services;
        }
    }
}
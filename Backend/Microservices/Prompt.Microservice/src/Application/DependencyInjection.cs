using Application.Behaviors;
using Application.Common.GeminiApi;
using Application.Consumers;
using FluentValidation;
using Infrastructure.Configs;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SharedLibrary.Common;
using Infrastructure.Repositories;
using MassTransit;
using SharedLibrary.Common.Messaging.Commands;

namespace Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            var assembly = typeof(DependencyInjection).Assembly;
            var sharedLibraryAssembly = typeof(SaveChangesCommandHandler).Assembly;
            services.AddMediatR(configuration =>
            {
                configuration.RegisterServicesFromAssembly(assembly);
                configuration.RegisterServicesFromAssembly(sharedLibraryAssembly);
            });
            services.AddHttpClient("GeminiClient",
                    client => { client.BaseAddress = new Uri("https://generativelanguage.googleapis.com/"); })
                .ConfigurePrimaryHttpMessageHandler(() =>
                {
                    var handler = new SocketsHttpHandler
                    {
                        EnableMultipleHttp2Connections = true
                    };
                    return handler;
                });
            services.AddStackExchangeRedisCache(options =>
            {
                var redisHost = Environment.GetEnvironmentVariable("REDIS_HOST") ?? "localhost";
                var redisPassword = Environment.GetEnvironmentVariable("REDIS_PASSWORD");

                var config = string.IsNullOrEmpty(redisPassword)
                    ? $"{redisHost}:6379"
                    : $"{redisHost}:6379,password={redisPassword}";

                options.Configuration = config;
                options.InstanceName = "FoodApp:";
            });
            using var serviceProvider = services.BuildServiceProvider();
            var config = serviceProvider.GetRequiredService<EnvironmentConfig>();
            services.AddMassTransit(busConfigurator =>
            {
                busConfigurator.AddConsumer<ProcessRatingConsumer>();
                busConfigurator.SetKebabCaseEndpointNameFormatter();
                busConfigurator.UsingRabbitMq((context, configurator) =>
                {
                    configurator.Host(new Uri($"rabbitmq://{config.RabbitMqHost}:{config.RabbitMqPort}/"), h =>
                    {
                        h.Username(config.RabbitMqUser);
                        h.Password(config.RabbitMqPassword);
                    });
                    configurator.ConfigureEndpoints(context);
                });
            });

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<GoogleSearchBuilder>();
            services.AddHttpContextAccessor();
            services.AddValidatorsFromAssembly(assembly);
            services.AddAutoMapper(assembly);
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>));
            services.AddValidatorsFromAssembly(assembly, includeInternalTypes: true);
            return services;
        }
    }
}
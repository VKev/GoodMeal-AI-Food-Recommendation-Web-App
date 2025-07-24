using System;
using Application.Sagas;
using SharedLibrary.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Infrastructure.Configs;
using Infrastructure.Repositories;
using Domain.Repositories;
using Elastic.Clients.Elasticsearch;
using Infrastructure.Common;
using MassTransit;
using SharedLibrary.Common;
using SharedLibrary.Common.Event;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IRestaurantRepository, RestaurantRepository>();
            services.AddScoped<IFoodRepository, FoodRepository>();
            services.AddScoped<IRestaurantRatingRepository, RestaurantRatingRepository>();
            services.AddScoped<IFoodElasticRepository, FoodElasticRepository>();
            services.AddScoped(typeof(SharedLibrary.Common.IRepository<>), typeof(Repository<>));
            services.AddSingleton<EnvironmentConfig>();
            
            // Register EventBuffer as a single scoped instance for all interfaces
            services.AddScoped<EventBuffer>();
            services.AddScoped<IEventBuffer>(sp => sp.GetRequiredService<EventBuffer>());
            services.AddScoped<IEventUnitOfWork>(sp => sp.GetRequiredService<EventBuffer>());
            services.AddScoped<IEventFlusher>(sp => sp.GetRequiredService<EventBuffer>());
            
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            using var serviceProvider = services.BuildServiceProvider();
            var logger = serviceProvider.GetRequiredService<ILogger<AutoScaffold>>();
            var config = serviceProvider.GetRequiredService<EnvironmentConfig>();
            var scaffold = new AutoScaffold(logger)
                .Configure(
                    config.DatabaseHost,
                    config.DatabasePort,
                    config.DatabaseName,
                    config.DatabaseUser,
                    config.DatabasePassword,
                    config.DatabaseProvider);

            scaffold.UpdateAppSettings();
            string solutionDirectory = Directory.GetParent(Directory.GetCurrentDirectory())?.FullName ?? "";
            if (solutionDirectory != null)
            {
                DotNetEnv.Env.Load(Path.Combine(solutionDirectory, ".env"));
            }

            services.AddMassTransit(busConfigurator =>
            {
                busConfigurator.SetKebabCaseEndpointNameFormatter();

                // Add consumers
                busConfigurator.AddConsumer<Application.Consumers.GetRestaurantByIdConsumer>();
                busConfigurator.AddConsumer<Application.Consumers.GetRestaurantsByIdsConsumer>();
                busConfigurator.AddConsumer<Application.Consumers.CreateRestaurantConsumer>();

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

            services.AddSingleton<ElasticsearchClient>(sp =>
            {
                var uri = new Uri(config.ElasticSearchHost);
                var settings = new ElasticsearchClientSettings(uri)
                    .DefaultIndex(config.ElasticSearchDefaultIndex);
                return new ElasticsearchClient(settings);
            });
            return services;
            
        }
    }
}
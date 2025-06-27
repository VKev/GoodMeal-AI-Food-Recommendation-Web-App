using SharedLibrary.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Infrastructure.Configs;
using Infrastructure.Repositories;
using Domain.Repositories;
using Infrastructure.Common;
using MassTransit;
using SharedLibrary.Common;
using Application.Sagas;
using Application.Consumers;
using SharedLibrary.Common.Event;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IBusinessRestaurantRepository, BusinessRestaurantRepository>();
            services.AddScoped<IBusinessRepository, BusinessRepository>();
            services.AddScoped<IRestaurantRepository, RestaurantRepository>();
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddSingleton<EnvironmentConfig>();
            
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
            services.AddMassTransit(busConfigurator => {
                busConfigurator.AddSagaStateMachine<RestaurantCreatingSaga, RestaurantCreatingSagaData>()
                    .RedisRepository(r =>
                    {
                        r.DatabaseConfiguration($"{config.RedisHost}:{config.RedisPort},password={config.RedisPassword}");
                        r.KeyPrefix = "businessRestaurant-creating-saga";
                        r.Expiry = TimeSpan.FromMinutes(10);
                    });
                
                busConfigurator.AddConsumer<CreateBusinessRestaurantConsumer>();
                busConfigurator.AddConsumer<GetAllBusinessesConsumer>();
                busConfigurator.AddConsumer<EnableBusinessConsumer>();
                busConfigurator.AddConsumer<DeactiveBusinessConsumer>();
                
                busConfigurator.SetKebabCaseEndpointNameFormatter();
                busConfigurator.UsingRabbitMq((context, configurator) =>{
                    configurator.Host(new Uri($"rabbitmq://{config.RabbitMqHost}:{config.RabbitMqPort}/"), h=>{
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
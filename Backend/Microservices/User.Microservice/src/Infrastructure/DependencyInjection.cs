using Application.Consumers;
using SharedLibrary.Utils;
using SharedLibrary.Configs;
using SharedLibrary.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Domain.Repositories;
using Infrastructure.Repositories;
using Infrastructure.Common;
using MassTransit;
using Application.Sagas;
using Application.Users.Consumers;
using SharedLibrary.Common.Event;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
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
                busConfigurator.AddConsumer<AuthenticationUserCreatedConsumer>();
                busConfigurator.AddConsumer<GetUserRolesConsumer>();
                busConfigurator.AddSagaStateMachine<UserCreatingSaga, UserCreatingSagaData>()
                    .RedisRepository(r =>
                    {
                        r.DatabaseConfiguration($"{config.RedisHost}:{config.RedisPort},password={config.RedisPassword}");
                        r.KeyPrefix = "user-creating-saga";
                        r.Expiry = TimeSpan.FromMinutes(10);
                    });
                busConfigurator.SetKebabCaseEndpointNameFormatter();
                busConfigurator.UsingRabbitMq((context, configurator) =>
                {
                    if (config.IsRabbitMqCloud)
                    {
                        configurator.Host(config.RabbitMqUrl);
                    }
                    else
                    {
                        configurator.Host(new Uri($"rabbitmq://{config.RabbitMqHost}:{config.RabbitMqPort}/"), h =>
                        {
                            h.Username(config.RabbitMqUser);
                            h.Password(config.RabbitMqPassword);
                        });
                    }
                    configurator.ConfigureEndpoints(context);
                });
            });
            return services;
        }
    }
}
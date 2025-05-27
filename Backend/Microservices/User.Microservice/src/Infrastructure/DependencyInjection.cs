using System;
using SharedLibrary.Utils;
using SharedLibrary.Configs;
using SharedLibrary.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Domain.Repositories;
using Infrastructure.Repositories;
using Application.Abstractions.UnitOfWork;
using Infrastructure.Common;
using MassTransit;
using Application.Sagas;
using Infrastructure.Context;

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
using SharedLibrary.Utils;
using SharedLibrary.Configs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Domain.Repositories;
using Infrastructure.Repositories;
using MassTransit;
using SharedLibrary.Contracts.Business;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddScoped<IAdminRepository, AdminRepository>();
            services.AddScoped<IAuthenticationRepository, AuthenticationRepository>();
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
                busConfigurator.AddRequestClient<GetAllBusinessesRequest>();
                busConfigurator.AddRequestClient<ActiveBusinessRequest>();
                busConfigurator.AddRequestClient<DeactiveBusinessRequest>();

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
using Domain.Repositories;
using SharedLibrary.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Infrastructure.Configs;
using Infrastructure.Repositories;
using Infrastructure.Common;
using Infrastructure.Services;
using MassTransit;
using SharedLibrary.Common;
using SharedLibrary.Common.Event;
using Application.Consumers;
using VNPAY.NET;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddSingleton<EnvironmentConfig>();

            services.AddScoped<IVnpay, Vnpay>();
            services.AddScoped<IVnpayRepository, VnpayRepository>();

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
                busConfigurator.AddConsumer<CreateSubscriptionPaymentUrlConsumer>();

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
            return services;
        }
    }
}
using System.Text;
using Application.Consumers;
using SharedLibrary.Utils;
using SharedLibrary.Configs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Domain.Repositories;
using Infrastructure.Repositories;
using MassTransit;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddScoped<IAuthRepository, AuthRepository>();
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
                busConfigurator.SetKebabCaseEndpointNameFormatter();

                busConfigurator.AddConsumer<GetUserStatusConsumer>();
                busConfigurator.AddConsumer<EnableUserConsumer>();
                busConfigurator.AddConsumer<DisableUserConsumer>();
                busConfigurator.AddConsumer<DeleteUserConsumer>();
                busConfigurator.AddConsumer<UpdateUserConsumer>();
                busConfigurator.AddConsumer<GetUserRolesConsumer>();
                busConfigurator.AddConsumer<AddUserRoleConsumer>();
                busConfigurator.AddConsumer<RemoveUserRoleConsumer>();
                busConfigurator.AddConsumer<SearchUsersConsumer>();

                busConfigurator.AddConsumer<BusinessActivatedEventConsumer>();
                busConfigurator.AddConsumer<BusinessDeactivatedEventConsumer>();

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

            var base64String = Environment.GetEnvironmentVariable("GOOGLE_CREDENTIAL_BASE64");
            if (base64String != null && !string.IsNullOrWhiteSpace(base64String) && base64String.Length > 0)
            {
                var jsonBytes = Convert.FromBase64String(base64String);
                var jsonString = Encoding.UTF8.GetString(jsonBytes);

                FirebaseApp.Create(new AppOptions()
                {
                    Credential = GoogleCredential.FromJson(jsonString)
                });
            }

            return services;
        }
    }
}
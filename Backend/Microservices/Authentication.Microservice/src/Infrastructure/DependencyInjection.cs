using System.Text;
using SharedLibrary.Utils;
using SharedLibrary.Configs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Domain.Repositories;
using Infrastructure.Repositories;
using Application.Sagas;
using Application.Consumers;
using MassTransit;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Domain.Services;
using Infrastructure.Services;
using SharedLibrary.Contracts.GetUserRoles;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ICustomJwtProvider, CustomJwtProvider>();
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
                busConfigurator.AddConsumer<ControlAccessCustomRoleClaimConsumer>();
                busConfigurator.AddConsumer<UserCreatedConsumer>();
                busConfigurator.AddConsumer<UserDeletedConsumer>();

                // Add RequestClient for GetUserRoles
                busConfigurator.AddRequestClient<GetUserRolesRequest>();

                busConfigurator.AddSagaStateMachine<AuthenticationUserCreatingSaga, AuthenticationUserCreatingSagaData>()
                    .RedisRepository(r =>
                    {
                        r.DatabaseConfiguration($"{config.RedisHost}:{config.RedisPort},password={config.RedisPassword}");
                        r.KeyPrefix = "authenticationUser-creating-saga";
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
            
            string base64String = Environment.GetEnvironmentVariable("GOOGLE_CREDENTIAL_BASE64");
            byte[] jsonBytes = Convert.FromBase64String(base64String);
            string jsonString = Encoding.UTF8.GetString(jsonBytes);
            
            Console.WriteLine(jsonString);
            
            FirebaseApp.Create(new AppOptions()
            {
                Credential = GoogleCredential.FromJson(jsonString)
            });

            return services;
        }
    }
}
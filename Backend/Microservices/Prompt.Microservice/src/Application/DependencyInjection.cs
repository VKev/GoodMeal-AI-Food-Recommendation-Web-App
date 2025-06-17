using Application.Behaviors;
using Application.Common.GeminiApi;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SharedLibrary.Common;
using Infrastructure.Repositories;

namespace Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            var assembly = typeof(DependencyInjection).Assembly;
            services.AddHttpClient("GeminiClient",
                client => { client.BaseAddress = new Uri("https://generativelanguage.googleapis.com/"); });
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddSingleton<GoogleSearchBuilder>();
            services.AddMediatR(configuration => configuration.RegisterServicesFromAssembly(assembly));
            services.AddValidatorsFromAssembly(assembly);
            services.AddAutoMapper(assembly);
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>));
            services.AddValidatorsFromAssembly(assembly, includeInternalTypes: true);
            return services;
        }
    }
}
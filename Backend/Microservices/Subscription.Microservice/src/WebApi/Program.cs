using Application;
using Infrastructure;
using Infrastructure.Context;
using SharedLibrary.Utils;
using SharedLibrary.Configs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Serilog;

string solutionDirectory = Directory.GetParent(Directory.GetCurrentDirectory())?.FullName ?? "";
if (solutionDirectory != null)
{
    DotNetEnv.Env.Load(Path.Combine(solutionDirectory, ".env"));
}

var builder = WebApplication.CreateBuilder(args);
var environment = builder.Environment;

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAuthorization();
builder.Services.AddHttpContextAccessor();

// Add CORS services
var corsOrigins = Environment.GetEnvironmentVariable("CORS_ALLOWED_ORIGINS")?.Split(',') ??
                  new[] { "http://localhost:3000" };
var allowCredentials = bool.Parse(Environment.GetEnvironmentVariable("CORS_ALLOW_CREDENTIALS") ?? "true");

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(corsOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod();

        if (allowCredentials)
            policy.AllowCredentials();
    });
});

builder.Host.UseSerilog((hostingContext, loggerConfiguration) => loggerConfiguration
    .ReadFrom.Configuration(hostingContext.Configuration));
builder.Services.AddLogging(loggingBuilder =>
{
    loggingBuilder.ClearProviders();
    loggingBuilder.AddSerilog();
});

builder.Services.ConfigureOptions<DatabaseConfigSetup>();
builder.Services.AddDbContext<SubscriptionContext>((serviceProvider, options) =>
{
    var databaseConfig = serviceProvider.GetService<IOptions<DatabaseConfig>>()!.Value;
    options.UseNpgsql(databaseConfig.ConnectionString, actions =>
    {
        actions.EnableRetryOnFailure(databaseConfig.MaxRetryCount);
        actions.CommandTimeout(databaseConfig.CommandTimeout);
    });
    if (environment.IsDevelopment())
    {
        options.EnableDetailedErrors(databaseConfig.EnableDetailedErrors);
        options.EnableSensitiveDataLogging(databaseConfig.EnableSensitiveDataLogging);
    }
});

builder.Services
    .AddApplication()
    .AddInfrastructure();


var app = builder.Build();

app.UseSwagger(c => { c.RouteTemplate = "api/subscription/swagger/{documentName}/swagger.json"; });
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/api/subscription/swagger/v1/swagger.json", "Subscription Microservice API V1");
    c.RoutePrefix = "api/subscription/swagger";
});
app.MapGet("/", context =>
{
    context.Response.Redirect("/api/subscription/swagger");
    return Task.CompletedTask;
});

app.UseSerilogRequestLogging();

app.UseCors();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

AutoScaffold.UpdateAppSettingsFile("appsettings.json", "default");
AutoScaffold.UpdateAppSettingsFile("appsettings.Development.json", "default");
using Application;
using Infrastructure;
using Infrastructure.Configs;
using Infrastructure.Context;
using SharedLibrary.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Serilog;
using WebApi.Configs;

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

builder.Host.UseSerilog((hostingContext, loggerConfiguration) => loggerConfiguration
    .ReadFrom.Configuration(hostingContext.Configuration));
builder.Services.AddLogging(loggingBuilder =>
{
    loggingBuilder.ClearProviders();
    loggingBuilder.AddSerilog();
});

builder.Services.ConfigureOptions<DatabaseConfigSetup>();
builder.Services.AddDbContext<PromptDbContext>((serviceProvider, options) =>
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

builder.Services.AddInfrastructure(builder.Configuration).AddApplication();

var app = builder.Build();
var logger = app.Services.GetRequiredService<ILogger<AutoScaffold>>();
var config = app.Services.GetRequiredService<EnvironmentConfig>();

var scaffold = new AutoScaffold(logger)
    .Configure(
        config.DatabaseHost,
        config.DatabasePort,
        config.DatabaseName,
        config.DatabaseUser,
        config.DatabasePassword,
        config.DatabaseProvider)
    .SetPaths(
        outputDir: "../Domain/Entities",
        contextDir: "Context",
        contextName: "PromptDbContext",
        msBuildPath: "Build/obj",
        projectPath: "../Infrastructure/Infrastructure.csproj"
    );

scaffold.UpdateAppSettings();
scaffold.Run();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapGet("/", context =>
    {
        context.Response.Redirect("/swagger");
        return Task.CompletedTask;
    });
}

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

AutoScaffold.UpdateAppSettingsFile("appsettings.json", "default");
AutoScaffold.UpdateAppSettingsFile("appsettings.Development.json", "default");
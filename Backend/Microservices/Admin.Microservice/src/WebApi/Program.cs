using Application;
using Domain.Repositories;
using Infrastructure;
using Infrastructure.Repositories;
using SharedLibrary.Utils;
using SharedLibrary.Configs;
using Serilog;
using WebApi.Middlewares;

string solutionDirectory = Directory.GetParent(Directory.GetCurrentDirectory())?.FullName ?? "";
DotNetEnv.Env.Load(Path.Combine(solutionDirectory, ".env"));

var builder = WebApplication.CreateBuilder(args);
var environment = builder.Environment;

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Host.UseSerilog((hostingContext, loggerConfiguration) => loggerConfiguration
    .ReadFrom.Configuration(hostingContext.Configuration));

builder.Services.ConfigureOptions<DatabaseConfigSetup>();
builder.Services
    .AddApplication()
    .AddInfrastructure();

var app = builder.Build();

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

app.MapControllers();
app.UseMiddleware<GetAuthenticatedUserMiddleware>();
app.Run();

AutoScaffold.UpdateAppSettingsFile("appsettings.json", "default");
AutoScaffold.UpdateAppSettingsFile("appsettings.Development.json", "default");
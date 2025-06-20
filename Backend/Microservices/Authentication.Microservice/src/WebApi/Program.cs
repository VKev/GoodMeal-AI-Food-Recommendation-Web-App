using Application;
using Infrastructure;
using SharedLibrary.Utils;
using SharedLibrary.Configs;
using Serilog;

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

    app.UseSwagger(c =>
    {
        c.RouteTemplate = "api/auth/swagger/{documentName}/swagger.json";
    });
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/api/auth/swagger/v1/swagger.json", "Authentication Microservice API V1");
        c.RoutePrefix = "api/auth/swagger";
    });
    app.MapGet("/", context =>
    {
        context.Response.Redirect("/api/auth/swagger");
        return Task.CompletedTask;
    });

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.MapControllers();
app.Run();

AutoScaffold.UpdateAppSettingsFile("appsettings.json", "default");
AutoScaffold.UpdateAppSettingsFile("appsettings.Development.json", "default");
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

// Add CORS services
var corsOrigins = Environment.GetEnvironmentVariable("CORS_ALLOWED_ORIGINS")?.Split(',') ?? new[] { "http://localhost:3000" };
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

builder.Services.ConfigureOptions<DatabaseConfigSetup>();
builder.Services
    .AddApplication()
    .AddInfrastructure();

var app = builder.Build();

    app.UseSwagger(c =>
    {
        c.RouteTemplate = "api/admin/swagger/{documentName}/swagger.json";
    });
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/api/admin/swagger/v1/swagger.json", "Admin Microservice API V1");
        c.RoutePrefix = "api/admin/swagger";
    });
    app.MapGet("/", context =>
    {
        context.Response.Redirect("/api/admin/swagger");
        return Task.CompletedTask;
    });

app.UseSerilogRequestLogging();

app.UseCors();
app.UseHttpsRedirection();

app.MapControllers();
app.Run();

AutoScaffold.UpdateAppSettingsFile("appsettings.json", "default");
AutoScaffold.UpdateAppSettingsFile("appsettings.Development.json", "default");
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using System.Text.Json;
using System.Text.Json.Nodes;
using src;
using Microsoft.OpenApi.Models;

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Build Ocelot configuration from environment variables
var ocelotConfig = BuildOcelotConfigFromEnvironment();

// Write the generated configuration to a temporary file
var tempOcelotPath = Path.Combine(builder.Environment.ContentRootPath, "ocelot.generated.json");
File.WriteAllText(tempOcelotPath, JsonSerializer.Serialize(ocelotConfig, new JsonSerializerOptions { WriteIndented = true }));

new Startup(builder.Configuration).ConfigureServices(builder.Services);

builder.Configuration.SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("ocelot.generated.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

// Add Swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "GoodMeal API Gateway", 
        Version = "v1",
        Description = "API Gateway for GoodMeal microservices architecture"
    });
});

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddTransient<AuthenticationHandler>();
builder.Services.AddHealthChecks();

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

builder.Services.AddOcelot(builder.Configuration).AddDelegatingHandler<AuthenticationHandler>(true);

var app = builder.Build();

// Clean up temporary files on shutdown
var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();
lifetime.ApplicationStopping.Register(() =>
{
    if (File.Exists(tempOcelotPath))
    {
        File.Delete(tempOcelotPath);
    }
});

// Configure Swagger
app.UseSwagger(c =>
{
    c.RouteTemplate = "swagger/{documentName}/swagger.json";
});

app.UseSwaggerUI(c =>
{
    // API Gateway Swagger
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API Gateway V1");
    
    // Add microservice Swagger endpoints dynamically based on environment variables
    var routeIndex = 0;
    while (true)
    {
        var upstreamPath = Environment.GetEnvironmentVariable($"OCELOT_ROUTES_{routeIndex}_UPSTREAM_PATH");
        if (string.IsNullOrEmpty(upstreamPath))
            break;

        // Extract service name from upstream path pattern: /api/ServiceName/{everything}
        if (upstreamPath.StartsWith("/api/") && upstreamPath.Contains("/{everything}"))
        {
            var serviceName = upstreamPath.Substring(5); // Remove "/api/"
            serviceName = serviceName.Substring(0, serviceName.IndexOf("/")); // Get service name before "/{everything}"
            var serviceNameLower = serviceName.ToLower();
            
            // Create proper display name (capitalize first letter)
            var displayName = char.ToUpper(serviceName[0]) + serviceName.Substring(1).ToLower() + " Microservice";
            
            // Add Swagger endpoint for this microservice
            c.SwaggerEndpoint($"/api/{serviceNameLower}/swagger/v1/swagger.json", displayName);
        }

        routeIndex++;
    }
    
    c.RoutePrefix = "swagger";
    c.DocumentTitle = "GoodMeal API Documentation";
    c.DefaultModelsExpandDepth(-1);
    c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
    c.DisplayRequestDuration();
    c.EnableTryItOutByDefault();
    c.EnableDeepLinking();
    c.ShowExtensions();
});

// Add root redirect to Swagger
app.MapGet("/", context =>
{
    context.Response.Redirect("/swagger");
    return Task.CompletedTask;
});

// Configure middleware pipeline
app.UseCors();
app.UseAuthentication();

// Add health check endpoint before Ocelot middleware
app.MapHealthChecks("/api/health");

// Use conditional middleware to bypass Ocelot for health checks and swagger
app.UseWhen(context => !context.Request.Path.StartsWithSegments("/api/health") && 
                      !context.Request.Path.StartsWithSegments("/swagger"), 
    appBuilder => appBuilder.UseOcelot().Wait());

await app.RunAsync();

static object BuildOcelotConfigFromEnvironment()
{
    var routes = new List<object>();
    var routeIndex = 0;
    var swaggerRoutes = new List<object>();

    // Read routes from environment variables
    while (true)
    {
        var upstreamPath = Environment.GetEnvironmentVariable($"OCELOT_ROUTES_{routeIndex}_UPSTREAM_PATH");
        if (string.IsNullOrEmpty(upstreamPath))
            break;

        var upstreamMethods = Environment.GetEnvironmentVariable($"OCELOT_ROUTES_{routeIndex}_UPSTREAM_METHODS")?.Split(',') ?? new[] { "Get" };
        var downstreamScheme = Environment.GetEnvironmentVariable($"OCELOT_ROUTES_{routeIndex}_DOWNSTREAM_SCHEME") ?? "http";
        var downstreamHost = Environment.GetEnvironmentVariable($"OCELOT_ROUTES_{routeIndex}_DOWNSTREAM_HOST") ?? "localhost";
        var downstreamPort = Environment.GetEnvironmentVariable($"OCELOT_ROUTES_{routeIndex}_DOWNSTREAM_PORT") ?? "80";
        var downstreamPath = Environment.GetEnvironmentVariable($"OCELOT_ROUTES_{routeIndex}_DOWNSTREAM_PATH") ?? upstreamPath;

        var route = new
        {
            UpstreamPathTemplate = upstreamPath,
            UpstreamHttpMethod = upstreamMethods,
            DownstreamScheme = downstreamScheme,
            DownstreamHostAndPorts = new[]
            {
                new
                {
                    Host = downstreamHost,
                    Port = downstreamPort
                }
            },
            DownstreamPathTemplate = downstreamPath
        };

        routes.Add(route);

        // Extract service name from upstream path for Swagger routes
        // Pattern: /api/Service/{everything} -> service name
        if (upstreamPath.StartsWith("/api/") && upstreamPath.Contains("/{everything}"))
        {
            var serviceName = upstreamPath.Substring(5); // Remove "/api/"
            serviceName = serviceName.Substring(0, serviceName.IndexOf("/")); // Get service name before "/{everything}"
            var serviceNameLower = serviceName.ToLower();

            // Create corresponding Swagger route
            swaggerRoutes.Add(new
            {
                UpstreamPathTemplate = $"/api/{serviceNameLower}/swagger/{{everything}}",
                UpstreamHttpMethod = new[] { "Get" },
                DownstreamScheme = downstreamScheme,
                DownstreamHostAndPorts = new[]
                {
                    new
                    {
                        Host = downstreamHost,
                        Port = downstreamPort
                    }
                },
                DownstreamPathTemplate = $"/api/{serviceNameLower}/swagger/{{everything}}"
            });
        }

        routeIndex++;
    }

    // Add the generated Swagger routes to the main routes list
    routes.AddRange(swaggerRoutes);

    // If no routes found in environment variables, provide a default configuration
    if (routes.Count == 0)
    {
        Console.WriteLine("No routes found in environment variables. Using fallback configuration.");
        routes.Add(new
        {
            UpstreamPathTemplate = "/api/{everything}",
            UpstreamHttpMethod = new[] { "Get", "Post", "Put", "Delete" },
            DownstreamScheme = "http",
            DownstreamHostAndPorts = new[]
            {
                new
                {
                    Host = "localhost",
                    Port = "5000"
                }
            },
            DownstreamPathTemplate = "/api/{everything}"
        });
    }

    var globalConfig = new
    {
        BaseUrl = Environment.GetEnvironmentVariable("OCELOT_GLOBAL_BASE_URL") ?? "http://localhost:8080"
    };

    return new
    {
        Routes = routes,
        GlobalConfiguration = globalConfig
    };
}
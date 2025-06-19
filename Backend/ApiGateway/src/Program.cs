using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using System.Text.Json;
using System.Text.Json.Nodes;
using src;

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

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddTransient<AuthenticationHandler>();
builder.Services.AddHealthChecks();
builder.Services.AddOcelot(builder.Configuration).AddDelegatingHandler<AuthenticationHandler>(true);

var app = builder.Build();

// Clean up temporary file on shutdown
var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();
lifetime.ApplicationStopping.Register(() =>
{
    if (File.Exists(tempOcelotPath))
    {
        File.Delete(tempOcelotPath);
    }
});

// Configure middleware pipeline
app.UseAuthentication();

// Add health check endpoint before Ocelot middleware
app.MapHealthChecks("/api/health");

// Use conditional middleware to bypass Ocelot for health checks
app.UseWhen(context => !context.Request.Path.StartsWithSegments("/api/health"), 
    appBuilder => appBuilder.UseOcelot().Wait());

await app.RunAsync();

static object BuildOcelotConfigFromEnvironment()
{
    var routes = new List<object>();
    var routeIndex = 0;

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
        routeIndex++;
    }

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
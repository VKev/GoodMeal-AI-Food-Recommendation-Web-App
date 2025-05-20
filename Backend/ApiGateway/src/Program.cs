using Microsoft.OpenApi.Models;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using System.Text.Json;
using System.Text.Json.Nodes;

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);
var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? builder.Environment.EnvironmentName;
var ocelotConfigPath = Path.Combine(builder.Environment.ContentRootPath, $"ocelot.{env}.json");

// Read and store the original JSON file content in memory
var originalJsonContent = File.ReadAllText(ocelotConfigPath);

// Parse and modify the JSON content
var jsonObject = JsonNode.Parse(originalJsonContent);

if (jsonObject != null)
{
    var routes = jsonObject["Routes"];
    if (routes is JsonArray)
    {
        foreach (var route in routes.AsArray())
        {
            if (route["DownstreamHostAndPorts"] is JsonArray downstreams)
            {
                foreach (var downstream in downstreams)
                {
                    // Handle 'Host'
                    if (downstream["Host"] is JsonValue hostValue)
                    {
                        var host = hostValue.GetValue<string>();
                        if (host.StartsWith("{") && host.EndsWith("}"))
                        {
                            var envVar = host.Trim('{', '}');
                            downstream["Host"] = Environment.GetEnvironmentVariable(envVar) ?? host;
                        }
                    }

                    // Handle 'Port' (can be number or string)
                    if (downstream["Port"] is JsonValue portValue)
                    {
                        if (portValue.TryGetValue(out int portInt)) // Port as integer
                        {
                            var envVar = Environment.GetEnvironmentVariable($"PORT_{portInt}");
                            if (envVar != null && int.TryParse(envVar, out int newPort))
                            {
                                downstream["Port"] = newPort;
                            }
                        }
                        else if (portValue.TryGetValue(out string portString)) // Port as string
                        {
                            if (portString.StartsWith("{") && portString.EndsWith("}"))
                            {
                                var envVar = portString.Trim('{', '}');
                                downstream["Port"] = int.Parse(Environment.GetEnvironmentVariable(envVar) ?? portString);
                            }
                        }
                    }
                }
            }
        }
    }

    // Save the modified JSON back to the file
    File.WriteAllText(ocelotConfigPath, jsonObject.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));
}

builder.Configuration.SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile($"ocelot.{env}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

builder.Services.AddOcelot(builder.Configuration);

var app = builder.Build();

// Restore the original JSON content during application shutdown
var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();
lifetime.ApplicationStopping.Register(() =>
{
    File.WriteAllText(ocelotConfigPath, originalJsonContent);
});

await app.UseOcelot();
app.Run();

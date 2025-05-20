using System;
using System.Diagnostics;
using System.IO;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace SharedLibrary.Utils
{
    public class AutoScaffold
    {
        private readonly ILogger _logger;

        public AutoScaffold(ILogger logger)
        {
            _logger = logger;
        }

        private string Host { get; set; } = "localhost";
        private int Port { get; set; } = 5432;
        private string Database { get; set; } = "defaultdb";
        private string Username { get; set; } = "postgres";
        private string Password { get; set; } = "password";
        private string Provider { get; set; } = "Npgsql.EntityFrameworkCore.PostgreSQL";
        private string OutputDir { get; set; } = "../Domain/Entities";
        private string ContextDir { get; set; } = "Context";
        private string ContextName { get; set; } = "MyDbContext";
        private string MsBuildProjectExtensionsPath { get; set; } = "Build/obj";
        private string ProjectPath { get; set; } = "../Infrastructure/Infrastructure.csproj";

        public AutoScaffold Configure(string host, int port, string database, string username, string password, string provider = "postgres")
        {
            Host = host;
            Port = port;
            Database = database;
            Username = username;
            Password = password;
            Provider = provider.ToLower() switch
            {
                "postgres" => "Npgsql.EntityFrameworkCore.PostgreSQL",
                "sqlserver" => "Microsoft.EntityFrameworkCore.SqlServer",
                _ => throw new ArgumentException("Unsupported provider. Use 'postgres' or 'sqlserver'.")
            };

            return this;
        }

        public AutoScaffold SetPaths(string outputDir, string contextDir, string contextName, string msBuildPath, string projectPath)
        {
            OutputDir = outputDir;
            ContextDir = contextDir;
            ContextName = contextName;
            MsBuildProjectExtensionsPath = msBuildPath;
            ProjectPath = projectPath;
            return this;
        }

        public static void UpdateAppSettingsFile(string fileName, string connectionString)
        {
            var appSettingsPath = Path.Combine(Directory.GetCurrentDirectory(), fileName);

            if (!File.Exists(appSettingsPath))
            {
                return;
            }

            try
            {
                var json = File.ReadAllText(appSettingsPath);
                var jsonObject = JObject.Parse(json);
                var connectionStrings = jsonObject["ConnectionStrings"] ?? new JObject();
                connectionStrings["DefaultConnection"] = connectionString;
                jsonObject["ConnectionStrings"] = connectionStrings;
                File.WriteAllText(appSettingsPath, jsonObject.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void UpdateAppSettings()
        {
            var connectionString = $"Host={Host};Port={Port};Database={Database};Username={Username};Password={Password}";
            UpdateAppSettingsFile("appsettings.json", connectionString);
            UpdateAppSettingsFile("appsettings.Development.json", connectionString);
        }

        public void Run()
        {
            if(Environment.GetEnvironmentVariable("IS_SCAFFOLDING") == "true")
                return;
            Environment.SetEnvironmentVariable("IS_SCAFFOLDING", "true");
            
            var arguments = $"ef dbcontext scaffold Name=DefaultConnection {Provider} --output-dir {OutputDir} --context-dir {ContextDir} --context {ContextName} --msbuildprojectextensionspath \"{MsBuildProjectExtensionsPath}\" --project {ProjectPath} --startup-project ../WebApi/WebApi.csproj --namespace Domain.Entities --context-namespace Infrastructure.Context --force --no-build";

            try
            {
                using var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "dotnet",
                        Arguments = arguments,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };

                process.OutputDataReceived += (sender, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                    {
                        _logger.LogInformation("[SCAFFOLD OUTPUT]: {Message}", e.Data);
                    }
                };

                process.ErrorDataReceived += (sender, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                    {
                        _logger.LogError("[SCAFFOLD OUTPUT]: {Message}", e.Data);
                    }
                };

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                process.WaitForExit();
                if (process.ExitCode != 0)
                {
                    throw new Exception("Scaffolding process failed. Check the logs for details.");
                }
                
                _logger.LogInformation("Scaffolding process completed successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Scaffolding process failed.");
            }
            finally
            {
                Environment.SetEnvironmentVariable("IS_SCAFFOLDING", null);
            }
        }
    }
} 
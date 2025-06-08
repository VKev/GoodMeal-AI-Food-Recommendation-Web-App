using System;
using System.Diagnostics;
using System.IO;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace SharedLibrary.Utils
{
    public class AutoMigration
    {
        private readonly ILogger _logger;
        
        public AutoMigration(ILogger logger)
        {
            _logger = logger;
        }

        private string MigrationName { get; set; } = "AutoMigration";
        private string MsBuildProjectExtensionsPath { get; set; } = "Build/obj";
        private string ProjectPath { get; set; } = "../Infrastructure/Infrastructure.csproj";
        private string StartupProject { get; set; } = "../WebApi/WebApi.csproj";
        // Connection string properties
        private string Host { get; set; } = "localhost";
        private int Port { get; set; } = 5432;
        private string Database { get; set; } = "defaultdb";
        private string Username { get; set; } = "postgres";
        private string Password { get; set; } = "password";


        private void ExecuteMigrationCommand(string command, string additionalArgs = "")
        {
            if (Environment.GetEnvironmentVariable("IS_MIGRATING") == "true")
            {
                return;
            }
            Environment.SetEnvironmentVariable("IS_MIGRATING", "true");
            _logger.LogInformation("Generating new migration");
            try
            {
                var arguments = $"ef migrations {command} {additionalArgs} " +
                              $"--msbuildprojectextensionspath \"{MsBuildProjectExtensionsPath}\" " +
                              $"--project {ProjectPath} " +
                              $"--startup-project {StartupProject}";

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
                        _logger.LogInformation("{Message}", e.Data);
                    }
                };

                process.ErrorDataReceived += (sender, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                    {
                        _logger.LogError("{Message}", e.Data);
                    }
                };

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                process.WaitForExit();
                if (process.ExitCode != 0)
                {
                    throw new Exception($"Migration {command} process failed. Check the logs for details.");
                }

                _logger.LogInformation("Migration {Command} completed successfully.", command);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Migration {Command} process failed.", command);
                throw;
            }
            finally
            {
                Environment.SetEnvironmentVariable("IS_MIGRATING", null);
            }
        }
        
        public void GenerateMigration()
        {
            ExecuteMigrationCommand("add", MigrationName+"_"+DateTime.Now.ToString("yyyyMMddHHmmss"));
        }

        public void ApplyMigration()
        {
            _logger.LogInformation("Applying pending migrations to the database");
            ExecuteMigrationCommand("update");
        }

        public void RollbackMigration(string targetMigration = "0")
        {
            _logger.LogInformation("Rolling back to migration: {TargetMigration}", targetMigration);
            ExecuteMigrationCommand("update", targetMigration);
        }
    }
} 
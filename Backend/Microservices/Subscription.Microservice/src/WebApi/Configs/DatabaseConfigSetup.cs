using Microsoft.Extensions.Options;
using SharedLibrary.Configs;

namespace WebApi.Configs;

public class DatabaseConfigSetup : IConfigureOptions<DatabaseConfig>
{
    private readonly IConfiguration _configuration;

    public DatabaseConfigSetup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void Configure(DatabaseConfig options)
    {
        var connectionString = _configuration.GetConnectionString("DefaultConnection");
        if (!string.IsNullOrEmpty(connectionString))
        {
            options.ConnectionString = connectionString;
        }

        options.MaxRetryCount = _configuration.GetValue<int>("Database:MaxRetryCount", 3);
        options.CommandTimeout = _configuration.GetValue<int>("Database:CommandTimeout", 30);
        options.EnableDetailedErrors = _configuration.GetValue<bool>("Database:EnableDetailedErrors", false);
        options.EnableSensitiveDataLogging = _configuration.GetValue<bool>("Database:EnableSensitiveDataLogging", false);
    }
} 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace WebApi.Configs
{
    public class DatabaseConfigSetup : IConfigureOptions<DatabaseConfig>
    {
        private readonly string ConfigurationSectionName = "DatabaseConfigurations";
        private readonly IConfiguration _configuration;

        public DatabaseConfigSetup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void Configure(DatabaseConfig options)
        {
            options.ConnectionString = _configuration.GetConnectionString("DefaultConnection")!;
            _configuration.GetSection(ConfigurationSectionName).Bind(options);

            // options.MaxRetryCount = _databaseConfig.MaxRetryCount;
            // options.CommandTimeout = _databaseConfig.CommandTimeout;
            // options.EnableDetailedErrors = _databaseConfig.EnableDetailedErrors;
            // options.EnableSensitiveDataLogging = _databaseConfig.EnableSensitiveDataLogging;
        }
    }
}
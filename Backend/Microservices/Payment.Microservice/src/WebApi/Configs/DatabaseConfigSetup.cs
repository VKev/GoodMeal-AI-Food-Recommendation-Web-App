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
        }
    }
}
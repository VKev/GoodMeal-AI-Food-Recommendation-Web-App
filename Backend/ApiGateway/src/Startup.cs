using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace src;

public class Startup
{
    private readonly IConfiguration _cfg;

    public Startup(IConfiguration configuration) => _cfg = configuration;

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddAuthentication().AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, x =>
        {
            x.Authority = _cfg["Authentication:Authority"];
            x.Audience = _cfg["Authentication:Audience"];
            x.TokenValidationParameters.ValidIssuer = _cfg["Authentication:ValidIssuer"];
        });
    }
}
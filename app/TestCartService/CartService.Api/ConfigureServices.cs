using CartService.Api.Constants;
using CartService.Application;
using CartService.Infrastructure;

namespace CartService.Api;

public static class ConfigureServices
{
    /// <summary>
    /// Method that adds app settings and checks if everything is set correctly.
    /// </summary>
    /// <param name="builder">Specified application builder.</param>
    public static void RegisterAppConfig(this WebApplicationBuilder builder)
    {
        var appConfig = builder.Configuration.Get<AppConfig>();

        if (appConfig is null)
        {
            throw new InvalidOperationException("AppConfig could not be loaded from configuration.");
        }

        builder.Services.Configure<ApplicationConfig>(builder.Configuration.GetSection(ConfigSectionConstants.Application));
        builder.Services.Configure<InfrastructureConfig>(builder.Configuration.GetSection(ConfigSectionConstants.Infrastructure));
    }
}
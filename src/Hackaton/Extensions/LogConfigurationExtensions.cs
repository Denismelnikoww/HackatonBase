using Hackaton;

namespace Web.Extensions;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

public static class LogConfigurationExtensions
{
    public static void LogConfigurationAsJson(this WebApplication app, bool showSensitiveData = false)
    {
        var logger = app.Services.GetRequiredService<ILogger<Program>>();
        var configuration = app.Services.GetRequiredService<IConfiguration>();

        var configDict = configuration.AsEnumerable()
            .Where(x => x.Value != null)
            .OrderBy(x => x.Key)
            .ToDictionary(x => x.Key, x => x.Value);

        var json = System.Text.Json.JsonSerializer.Serialize(configDict, new System.Text.Json.JsonSerializerOptions
        {
            WriteIndented = true
        });

        logger.LogInformation("Full Configuration:\n{Config}", json);
    }
}
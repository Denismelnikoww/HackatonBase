using Infrastructure.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace Infrastructure.Extensions
{
    public static class SerilogExtension
    {
        public static IHostBuilder UseCustomLogging(this IHostBuilder hostBuilder)
        {
            hostBuilder.UseSerilog((context, config) =>
            {
                var settings = context.Configuration.GetSection("LoggingSettings").Get<LoggingSettings>();

                settings ??= new LoggingSettings();

                var logLevel = Enum.Parse<LogEventLevel>(settings.LogLevel);
                config.MinimumLevel.Is(logLevel);

                if (settings.ConsoleEnabled)
                {
                    config.WriteTo.Console(
                        outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}"
                    );
                }
                if (settings.FileEnabled)
                {
                    config.WriteTo.File(
                        path: settings.LogPath,
                        rollingInterval: RollingInterval.Day,
                        retainedFileCountLimit: 7,
                        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}"
                    );
                }

                config.Enrich.FromLogContext();
            });

            return hostBuilder;
        }
    }
}

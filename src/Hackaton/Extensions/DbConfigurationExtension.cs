using Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Web.Extensions
{
    public static class DbConfigurationExtension
    {
        public static IHostApplicationBuilder AddDb(this IHostApplicationBuilder builder)
        {
            builder.Services.AddDbContext<UserDbContext>(options =>
                 options.UseNpgsql(
                   builder.Configuration.GetDbConnectionString(),
                     npgsqlOptions =>
                     {
                         npgsqlOptions.MigrationsHistoryTable("__EFMigrationsHistory", "users");
                         npgsqlOptions.MigrationsAssembly("Infrastructure");
                     }
             ));
            return builder;
        }

        public static string GetDbConnectionString(this IConfiguration configuration)
        {
            var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD") ??
               configuration["DB_PASSWORD"];

            if (string.IsNullOrEmpty(dbPassword))
            {
                throw new InvalidOperationException("DB_PASSWORD not configured");
            }

            var connectionStringTemplate = configuration.GetConnectionString("Database");
            if (string.IsNullOrEmpty(connectionStringTemplate))
            {
                throw new InvalidOperationException("Database connection string not configured");
            }

            return string.Format(connectionStringTemplate, dbPassword);
        }
    }
}

using Infrastructure.DbContexts;
using Infrastructure.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Web.Extensions
{
    public static class DbConfigurationExtension
    {
        public static IHostApplicationBuilder AddDb(this IHostApplicationBuilder builder)
        {
            var connectionString = builder.Services.BuildServiceProvider()
              .GetRequiredService<IOptions<ConnectionStringsOptions>>().Value;

            builder.Services.AddDbContext<UserDbContext>(options =>
                 options.UseNpgsql(
                   connectionString.DatabaseConnectionString,
                     npgsqlOptions =>
                     {
                         npgsqlOptions.MigrationsHistoryTable("__EFMigrationsHistory", "users");
                         npgsqlOptions.MigrationsAssembly("Infrastructure");
                     }
             ));
            return builder;
        }
    }
}

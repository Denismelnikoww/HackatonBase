using Hangfire;
using Hangfire.PostgreSql;
using Infrastructure.BackgroundJobs;

namespace Web.Extensions
{
    public static class HangfireConfigurationExtension
    {
        public static IHostApplicationBuilder AddHangfire(
    this IHostApplicationBuilder builder)
        {
            builder.Services.AddHangfire(config =>
                config.UsePostgreSqlStorage(
                    builder.Configuration.GetDbConnectionString()));

            builder.Services.AddHangfireServer();

            builder.Services.AddScoped<IBackgroundJobService, HangfireJobService>();

            return builder;
        }

        public static IApplicationBuilder UseHangfireDashboard(
            this IApplicationBuilder app)
        {
            var options = new DashboardOptions
            {
                DashboardTitle = "Jobs Dashboard",
                DarkModeEnabled = true,
                Authorization = new[] { new HangfireAuthorizationFilter() }
            };

            app.UseHangfireDashboard("/jobs", options);

            return app;
        }
    }
}

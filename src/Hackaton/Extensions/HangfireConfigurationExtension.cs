using Hangfire;
using Hangfire.Dashboard;
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

        public static IApplicationBuilder UseHangfireDashboard(this IApplicationBuilder app,
            bool useAuthorization = false)
        {
            var options = new DashboardOptions
            {
                DashboardTitle = "Jobs Dashboard",
                DarkModeEnabled = true,
                Authorization = useAuthorization
                    ? new[] { new HangfireAuthorizationFilter() }
                    : Array.Empty<IDashboardAuthorizationFilter>()
            };

            app.UseHangfireDashboard("/jobs", options);

            return app;
        }
    }
}

using Hangfire.Dashboard;

namespace Infrastructure.BackgroundJobs
{

    public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            return true;

            var httpContext = context.GetHttpContext();

            if (httpContext.User?.Identity?.IsAuthenticated != true)
                return false;

            if (httpContext.User.IsInRole("Admin") || httpContext.User.IsInRole("HangfireAccess"))
                return true;

            return false;
        }
    }
}
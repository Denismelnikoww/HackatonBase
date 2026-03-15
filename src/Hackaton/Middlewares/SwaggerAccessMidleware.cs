
namespace Web.Middlewares
{
    public class SwaggerAccessMiddleware(RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path.StartsWithSegments("/swagger"))
            {
                if (!context.User.Identity?.IsAuthenticated ?? true)
                {
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync("Unauthorized. Please login to access Swagger.");
                    return;
                }

                if (!context.User.IsInRole("Admin"))
                {
                    context.Response.StatusCode = 403;
                    await context.Response.WriteAsync("Access Denied. This area is restricted to administrators only.");
                    return;
                }
            }

            await next(context);
        }
    }
}
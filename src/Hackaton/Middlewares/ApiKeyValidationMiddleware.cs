namespace Web.Middlewares
{
    public class ApiKeyValidationMiddleware(RequestDelegate next, IServiceProvider serviceProvider)
    {
        private const string API_KEY_HEADER = "X-API-Key";

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path.StartsWithSegments("/api"))
            {
                if (!context.Request.Headers.TryGetValue(API_KEY_HEADER, out var extractedApiKey))
                {
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync("API Key missing");
                    return;
                }

                //var apiKeyService = serviceProvider.GetRequiredService<IApiKeyService>();
                //var isValid = await apiKeyService.ValidateApiKeyAsync(extractedApiKey!);

                //if (!isValid)
                {
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync("Invalid API Key");
                    return;
                }
            }

            await next(context);
        }
    }
}

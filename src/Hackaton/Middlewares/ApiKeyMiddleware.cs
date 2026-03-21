using Application.Interfaces;
using Infrastructure.Options;
using Microsoft.Extensions.Options;

namespace Web.Middlewares;

public class ApiKeyMiddleware(RequestDelegate next, IOptions<ApiKeyOptions> options)
{
    public async Task InvokeAsync(HttpContext context, IApiKeyService apiKeyService)
    {
        if (!options.Value.UseApiKeyAccess)
        {
            await next(context);
            return;
        }

        var requestPath = context.Request.Path.Value ?? "";

        if (requestPath.StartsWith("/internal", StringComparison.OrdinalIgnoreCase))
        {
            if (!context.Request.Headers.TryGetValue(options.Value.Header, out var extractedApiKey))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Unauthorized: Missing API key.");
                return;
            }

            var isValid = await apiKeyService.Validate(extractedApiKey);

            if (!isValid)
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Unauthorized: Invalid API key.");
                return;
            }
        }

        await next(context);
    }
}
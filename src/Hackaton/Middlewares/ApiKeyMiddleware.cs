using Application.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Web.Middlewares;

public class ApiKeyMiddleware
{
    private readonly RequestDelegate _next;

    public ApiKeyMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IApiKeyService apiKeyService)
    {
        var requestPath = context.Request.Path.Value ?? "";

        if (requestPath.StartsWith("/internal", StringComparison.OrdinalIgnoreCase))
        {
            if (!context.Request.Headers.TryGetValue("X-API-Key", out var extractedApiKey))
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

        await _next(context);
    }
}
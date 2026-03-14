using System.Text.Json;

namespace Web.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            _logger.LogError(exception, "Произошла ошибка при обработке запроса {Method} {Path}",
                context.Request.Method,
                context.Request.Path);

            var response = context.Response;
            response.ContentType = "application/json";

            var (statusCode, message) = GetStatusCodeAndMessage(exception);
            response.StatusCode = statusCode;

            var errorResponse = new
            {
                StatusCode = statusCode,
                Message = message,
                Timestamp = DateTime.UtcNow,
                Path = context.Request.Path
            };

            var json = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await response.WriteAsync(json);
        }

        private static (int statusCode, string message) GetStatusCodeAndMessage(Exception exception)
        {
            return exception switch
            {
                UnauthorizedAccessException => (401, "Не авторизован"),
                KeyNotFoundException => (404, "Ресурс не найден"),
                ArgumentException => (400, exception.Message),
                InvalidOperationException => (400, exception.Message),
                _ => (500, "Внутренняя ошибка сервера")
            };
        }
    }
}

using Infrastructure.Extensions;
using Web.Extensions;
using Web.Middlewares;
using Web.Middlewares.Web.Middlewares;

namespace Hackaton
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Configuration.AddEnvironmentVariables();
            builder.Configuration.AddKeyPerFile("/run/secrets", optional: true, reloadOnChange: true);
            builder.AddOptions();
            builder.ValidateOptions();

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddXmlCommentsToSwagger();

            builder.AddAuth();

            builder.AddDb();
            builder.AddRedis();

            builder.AddHangfire();

            builder.Services.AddServices();
            builder.Services.AddBackgroundJobs();
            builder.Host.UseCustomLogging();
            builder.AddClaimsPrincipalExtension();
            builder.AddHttpClients();
            builder.Services.AddCoreAdmin(builder.Environment.IsDevelopment() ? string.Empty : "Admin");

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigin",
                    policy =>
                    {
                        policy.WithOrigins("https://shkets.ru",
                                "http://localhost:5173", "http://localhost:5000",
                                "http://192.168.31.223:5273", "http://192.168.31.159:7225")
                            .AllowCredentials()
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    });
            });

            var app = builder.Build();

            app.UseMiddleware<OptionsMiddleware>();

            app.UseCors("AllowSpecificOrigin");

            app.UseRouting();

            app.UseMiddleware<ExceptionHandlingMiddleware>();
            // app.UseMiddleware<ApiKeyMiddleware>();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseHangfireDashboard(builder.Environment.IsProduction());

            //if (builder.Environment.IsProduction())
            //    app.UseMiddleware<SwaggerAccessMiddleware>();

            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.MapControllers();
            app.MapDefaultControllerRoute();

            app.UseCoreAdminCustomTitle("Shkets Admin");
            app.UseCoreAdminCustomUrl("admin");

            app.ApplyMigrations();

            app.Run();
        }
    }
}
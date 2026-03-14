using Infrastructure.Extensions;
using Web.Extensions;
using Web.Middlewares;

namespace Hackaton
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Configuration.AddEnvironmentVariables();
            builder.Configuration.AddKeyPerFile("/run/secrets", optional: true);
            builder.AddOptions();
            builder.ValidateOptions();

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.AddHangfire();

            builder.AddAuth();

            builder.AddDb();
            builder.AddRedis();

            builder.Services.AddServices();
            builder.Host.UseCustomLogging();
            builder.AddClaimsPrincipalExtension();
            builder.Services.AddCoreAdmin(builder.Environment.IsDevelopment() ? string.Empty : "Admin");

            var app = builder.Build();

            app.UseMiddleware<ExceptionHandlingMiddleware>();

            app.UseAuthorization();
            app.UseAuthentication();

            //if (builder.Environment.IsProduction())
            //    app.UseMiddleware<SwaggerAccessMiddleware>();

            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.MapControllers();
            app.MapDefaultControllerRoute();

            app.UseCoreAdminCustomTitle("admin");

            app.ApplyMigrations();

            app.UseHangfireDashboard(builder.Environment.IsProduction());

            app.Run();
        }
    }
}

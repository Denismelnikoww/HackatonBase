using Infrastructure.Extensions;
using Web.Extensions;
using Web.Middlewares;

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
            builder.Host.UseCustomLogging();
            builder.AddClaimsPrincipalExtension();
            builder.AddHttpClients();
            builder.Services.AddCoreAdmin(builder.Environment.IsDevelopment() ? string.Empty : "Admin");

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigin",
                    policy =>
                    {
                        policy.WithOrigins("https://localhost:7225/swagger/index.html")
                              .AllowCredentials()
                              .AllowAnyHeader()
                              .AllowAnyMethod();
                    });
            });


            var app = builder.Build();

            app.UseMiddleware<ExceptionHandlingMiddleware>();

            app.UseCors("AllowSpecificOrigin");

            app.UseAuthentication();
            app.UseAuthorization();

            //if (builder.Environment.IsProduction())
            //    app.UseMiddleware<SwaggerAccessMiddleware>();

            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.MapControllers();
            app.MapDefaultControllerRoute();

            app.UseCoreAdminCustomTitle("Shkets");

            app.ApplyMigrations();

            app.UseHangfireDashboard(builder.Environment.IsProduction());

            app.Run();
        }
    }
}

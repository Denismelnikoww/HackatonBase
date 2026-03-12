using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Routing;
using Infrastructure.Extensions;
using Web.Extensions;

namespace Hackaton
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Configuration.AddEnvironmentVariables();
            builder.Configuration.AddKeyPerFile("/run/secrets", optional: true);

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.AddHangfire();

            builder.AddAuth();

            builder.AddDb();

            builder.Services.AddServices();

            builder.Host.UseCustomLogging();

            builder.Services.AddCoreAdmin();

            var app = builder.Build();

            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.MapDefaultControllerRoute();
            app.UseCoreAdminCustomTitle("admin");
            app.UseAuthorization();
            app.UseAuthentication();
            app.MapControllers();
            app.ApplyMigrations();
            app.UseHangfireDashboard();

            app.Run();
        }
    }
}

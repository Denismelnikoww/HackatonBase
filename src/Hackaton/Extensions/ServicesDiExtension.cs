using Application.Interfaces;
using Application.Services;
using Infrastructure.Auth;
using Infrastructure.Interfaces;
using Infrastructure.Services;

namespace Web.Extensions
{
    public static class ServicesDiExtension
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddSingleton<ILoadService, LoadService>();

            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<IJwtProvider, JwtProvider>();

            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserService,UserService>();

            return services;
        }

    }
}

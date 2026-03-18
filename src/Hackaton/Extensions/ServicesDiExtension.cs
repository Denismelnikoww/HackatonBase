using Application.Interfaces;
using Application.Services;
using Infrastructure.Auth;
using Infrastructure.Email;
using Infrastructure.Interfaces;
using Infrastructure.Mistral;
using Infrastructure.Services;

namespace Web.Extensions
{
    public static class ServicesDiExtension
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddSingleton<ILoadService, LoadService>();

            services.AddSingleton<IRedisCacheService, RedisCacheService>();

            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<IJwtProvider, JwtProvider>();

            services.AddScoped<IEmailTemplateBuilder, EmailTemplateBuilder>();
            services.AddScoped<IEmailService, EmailService>();

            services.AddScoped<IMistralService, MistralService>();

            services.AddScoped<IResetPasswordService, ResetPasswordService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserService,UserService>();

            return services;
        }

    }
}

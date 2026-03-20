using Application.Interfaces;
using Application.Services;
using Application.Services.Internal;
using Infrastructure.Auth;
using Infrastructure.BackgroundJobs.Jobs;
using Infrastructure.BackgroundJobs.Jobs.Interfaces;
using Infrastructure.Email;
using Infrastructure.Interfaces;
using Infrastructure.Services;

namespace Web.Extensions
{
    public static class DiExtension
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddSingleton<IRedisCacheService, RedisCacheService>();

            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<IJwtProvider, JwtProvider>();

            services.AddScoped<IEmailTemplateBuilder, EmailTemplateBuilder>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IEmailConfirmService, EmailConfirmService>();

            services.AddScoped<IResetPasswordByEmailService, ResetPasswordByEmailService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IApiKeyService, ApiKeyService>();
            services.AddScoped<IQrService, QrService>();

            return services;
        }

        public static IServiceCollection AddBackgroundJobs(this IServiceCollection services)
        {
            services.AddScoped<IEmailBackgroundJob, EmailBackgroundJob>();

            return services;
        }
    }
}
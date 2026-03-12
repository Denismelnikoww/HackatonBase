using Infrastructure.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Web.Extensions
{
    public static class AuthConfigurationExtension
    {
        public static IHostApplicationBuilder AddAuth(this IHostApplicationBuilder builder)
        {
            var settings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();

            if (settings == null)
            {
                throw new InvalidOperationException("JwtSettings section is missing in configuration");
            }

            var secret = Environment.GetEnvironmentVariable("JWT_SECRET") ??
                builder.Configuration["JWT_SECRET"];

            if (string.IsNullOrEmpty(secret))
            {
                throw new InvalidOperationException("JWT_SECRET environment variable is not set");
            }

            settings.Secret = secret;

            builder.Services.Configure<JwtSettings>(options =>
            {
                options.AccessCookieName = settings.AccessCookieName;
                options.RefreshCookieName = settings.RefreshCookieName;
                options.UserIdCookieName = settings.UserIdCookieName;
                options.SessionCookieName = settings.SessionCookieName;
                options.Issuer = settings.Issuer;
                options.Audience = settings.Audience;
                options.AccessTokenExpirationMinutes = settings.AccessTokenExpirationMinutes;
                options.RefreshTokenExpirationDays = settings.RefreshTokenExpirationDays;
                options.Secret = secret;
            });

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, opt =>
                {
                    opt.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            context.Token = context.Request.Cookies[settings.AccessCookieName];
                            return Task.CompletedTask;
                        }
                    };

                    opt.TokenValidationParameters = new()
                    {
                        ValidateAudience = true,
                        ValidAudiences = new[] { settings.Audience },

                        ValidateIssuer = true,
                        ValidIssuers = new[] { settings.Issuer },

                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.Secret)),
                        RequireSignedTokens = true,

                        RequireExpirationTime = true,
                        ValidateLifetime = true,
                    };
                });
            builder.Services.AddAuthorization();

            return builder;
        }
    }
}

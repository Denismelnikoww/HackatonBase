using Infrastructure.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace Web.Extensions
{
    public static class ClaimsPrincipalExtension
    {
        private static JwtSettings _jwtSettings = null!;

        public static IHostApplicationBuilder AddClaimsPrincipalExtension(this IHostApplicationBuilder builder)
        {
            _jwtSettings = builder.Services.BuildServiceProvider()
                .GetRequiredService<IOptions<JwtSettings>>().Value;

            return builder;
        }

        public static string GetUserId(this ClaimsPrincipal user)
        {
            return user.FindFirstValue(_jwtSettings.UserIdCookieName);
        }

        public static string GetSessionId(this ClaimsPrincipal user)
        {
            return user.FindFirstValue(_jwtSettings.SessionCookieName);
        }
    }
}

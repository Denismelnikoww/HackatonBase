using StackExchange.Redis;

namespace Web.Extensions
{
    public static class RedisConfigurationExtension
    {
        public static IHostApplicationBuilder AddRedis(this IHostApplicationBuilder builder)
        {
            builder.Services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = builder.Configuration.GetRedisConnectionString();
                options.InstanceName = builder.Configuration["Redis:InstanceName"] ?? "Hackaton:";
            });

            builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                var configuration = builder.Configuration.GetRedisConnectionString();
                return ConnectionMultiplexer.Connect(configuration);
            });

            return builder;
        }

        public static string GetRedisConnectionString(this IConfiguration configuration)
        {
            var redisPassword = Environment.GetEnvironmentVariable("redis_password") ??
                configuration["redis_password"];

            if (string.IsNullOrEmpty(redisPassword))
            {
                throw new InvalidOperationException("redis_password is not set");
            }

            var connectionStringTemplate = configuration.GetConnectionString("Redis");
            if (string.IsNullOrEmpty(connectionStringTemplate))
            {
                throw new InvalidOperationException("redis_connections string is not set");
            }

            return string.Format(connectionStringTemplate, redisPassword);
        }
    }
}
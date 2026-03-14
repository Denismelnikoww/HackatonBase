using Infrastructure.Options;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace Web.Extensions
{
    public static class RedisConfigurationExtension
    {
        public static IHostApplicationBuilder AddRedis(this IHostApplicationBuilder builder)
        {
            var connectionString = builder.Services.BuildServiceProvider()
                .GetRequiredService<IOptions<ConnectionStringsOptions>>().Value;

            builder.Services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = connectionString.RedisConnectionString;
                options.InstanceName = connectionString.RedisInstanceName;
            });

            builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                return ConnectionMultiplexer.Connect(connectionString.DatabaseConnectionString);
            });

            return builder;
        }
    }
}
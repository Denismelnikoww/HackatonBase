using Infrastructure.Options;
using Microsoft.Extensions.Options;

namespace Web.Extensions
{
    public static class HttpClientConfigurationExtension
    {
        public static IHostApplicationBuilder AddHttpClients(this IHostApplicationBuilder builder)
        {
            var mistralOptions = builder.Services.BuildServiceProvider()
                .GetRequiredService<IOptions<MistralOptions>>().Value;

            builder.Services.AddHttpClient("Mistral", client =>
            {
                client.BaseAddress = new Uri(mistralOptions.BaseUrl);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {mistralOptions.ApiKey}");
            });

            return builder;
        }
    }
}

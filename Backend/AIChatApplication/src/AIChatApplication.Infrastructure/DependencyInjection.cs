using AIChatApplication.Application.Interface;
using AIChatApplication.Infrastructure.Ollama;
using AIChatApplication.Infrastructure.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace AIChatApplication.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddOptions<OllamaOptions>()
            .Bind(configuration.GetSection(OllamaOptions.SectionName))
            .Validate(options => Uri.TryCreate(
                options.BaseUrl, UriKind.Absolute, out var uri)
                && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps),
                "Ollama:BaseUrl must be a valid HTTP or HTTPS URL.")
            .Validate(options => !string.IsNullOrWhiteSpace(options.Model),
                "Ollama:Model is required.")
            .Validate(options => !string.IsNullOrWhiteSpace(options.SystemPrompt),
                "Ollama:SystemPrompt is required.")
            .Validate(options => options.TimeoutSeconds > 0,
                "Ollama:TimeoutSeconds must be greater than zero.")
            .ValidateOnStart();

        services.AddHttpClient<IChatProvider, OllamaChatProvider>((provider, client) =>
        {
            var options = provider.GetRequiredService<IOptions<OllamaOptions>>().Value;
            client.BaseAddress = new Uri(options.BaseUrl.TrimEnd('/') + "/");
            client.Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds);
        });

        services.AddSingleton<IConversationStore, InMemoryConversationStore>();

        return services;
    }
}

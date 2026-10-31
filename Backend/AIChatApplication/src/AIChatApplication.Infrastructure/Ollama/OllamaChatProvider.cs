using System.Net.Http.Json;
using AIChatApplication.Application.Interface;
using AIChatApplication.Domain.Conversations;
using Microsoft.Extensions.Options;

namespace AIChatApplication.Infrastructure.Ollama;

public sealed class OllamaChatProvider : IChatProvider
{
    private readonly HttpClient _httpClient;
    private readonly OllamaOptions _options;

    public OllamaChatProvider(
        HttpClient httpClient,
        IOptions<OllamaOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }

    public async Task<string> GetReplyAsync(
        IReadOnlyList<ChatMessage> messages,
        CancellationToken cancellationToken)
    {
        var requestMessages = new List<OllamaMessage>
        {
            new("system", _options.SystemPrompt)
        };

        requestMessages.AddRange(messages.Select(
            message => new OllamaMessage(message.Role, message.Content)));

        var request = new OllamaRequest(
            _options.Model, requestMessages, Stream: false);

        using var response = await _httpClient.PostAsJsonAsync(
            "api/chat", request, cancellationToken);

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<OllamaResponse>(
            cancellationToken: cancellationToken);

        var answer = result?.Message?.Content;

        if (string.IsNullOrWhiteSpace(answer))
        {
            throw new InvalidOperationException(
                "Ollama returned an empty response.");
        }

        return answer;
    }

    private sealed record OllamaRequest(
        string Model,
        List<OllamaMessage> Messages,
        bool Stream);

    private sealed record OllamaMessage(string Role, string Content);

    private sealed record OllamaResponse(OllamaResponseMessage? Message);

    private sealed record OllamaResponseMessage(string? Role, string? Content);
}

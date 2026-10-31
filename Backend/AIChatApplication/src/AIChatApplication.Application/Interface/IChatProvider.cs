using AIChatApplication.Domain.Conversations;

namespace AIChatApplication.Application.Interface;

public interface IChatProvider
{
    Task<string> GetReplyAsync(
        IReadOnlyList<ChatMessage> messages,
        CancellationToken cancellationToken);
}

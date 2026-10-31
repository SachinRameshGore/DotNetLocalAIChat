using System.Collections.Concurrent;
using AIChatApplication.Application.Interface;
using AIChatApplication.Domain.Conversations;

namespace AIChatApplication.Infrastructure.Persistence;

public sealed class InMemoryConversationStore : IConversationStore
{
    private readonly ConcurrentDictionary<string, ConversationState> _conversations = new();

    public ConversationState GetOrCreate(string conversationId)
    {
        return _conversations.GetOrAdd(
            conversationId, _ => new ConversationState());
    }
}

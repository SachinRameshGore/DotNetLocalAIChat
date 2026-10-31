namespace AIChatApplication.Domain.Conversations;

public sealed class ConversationState
{
    public SemaphoreSlim Gate { get; } = new(1, 1);

    public List<ChatMessage> Messages { get; } = [];
}

using AIChatApplication.Application.Interface;
using AIChatApplication.Domain.Conversations;

namespace AIChatApplication.Application.Chats;

public sealed class ChatService
{
    private const int MaxHistoryMessages = 20;

    private readonly IChatProvider _chatProvider;
    private readonly IConversationStore _conversationStore;

    public ChatService(
        IChatProvider chatProvider,
        IConversationStore conversationStore)
    {
        _chatProvider = chatProvider;
        _conversationStore = conversationStore;
    }

    public async Task<ChatReply> AskAsync(
        string? conversationId,
        string message,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            throw new ArgumentException(
                "Message cannot be empty.", nameof(message));
        }

        var id = string.IsNullOrWhiteSpace(conversationId)
            ? Guid.NewGuid().ToString("N")
            : conversationId.Trim();

        var conversation = _conversationStore.GetOrCreate(id);

        await conversation.Gate.WaitAsync(cancellationToken);

        try
        {
            var messages = conversation.Messages.ToList();
            messages.Add(new ChatMessage("user", message));

            var answer = await _chatProvider.GetReplyAsync(
                messages, cancellationToken);

            if (string.IsNullOrWhiteSpace(answer))
            {
                throw new InvalidOperationException(
                    "The AI provider returned an empty response.");
            }

            conversation.Messages.Add(new ChatMessage("user", message));
            conversation.Messages.Add(new ChatMessage("assistant", answer));

            if (conversation.Messages.Count > MaxHistoryMessages)
            {
                conversation.Messages.RemoveRange(
                    0, conversation.Messages.Count - MaxHistoryMessages);
            }

            return new ChatReply(id, answer);
        }
        finally
        {
            conversation.Gate.Release();
        }
    }
}

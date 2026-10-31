using AIChatApplication.Domain.Conversations;

namespace AIChatApplication.Application.Interface;

public interface IConversationStore
{
    ConversationState GetOrCreate(string conversationId);
}

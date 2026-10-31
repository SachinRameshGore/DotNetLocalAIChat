using System.ComponentModel.DataAnnotations;

namespace AIChatApplication.Api.Contracts;

public sealed class ChatRequest
{
    [Required]
    [StringLength(20000, MinimumLength = 1)]
    public string Message { get; set; } = string.Empty;

    [StringLength(100)]
    public string? ConversationId { get; set; }
}

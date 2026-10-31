using AIChatApplication.Api.Contracts;
using AIChatApplication.Application.Chats;
using Microsoft.AspNetCore.Mvc;

namespace AIChatApplication.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class ChatController(ChatService chatService) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<ChatReply>> Post(
        [FromBody] ChatRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Message))
        {
            return BadRequest(new { error = "Message is required." });
        }

        try
        {
            var reply = await chatService.AskAsync(
                request.ConversationId, request.Message, cancellationToken);

            return Ok(reply);
        }
        catch (HttpRequestException)
        {
            return StatusCode(StatusCodes.Status503ServiceUnavailable,
                new { error = "The local AI service is unavailable." });
        }
        catch (TaskCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            return StatusCode(StatusCodes.Status504GatewayTimeout,
                new { error = "The AI service took too long to respond." });
        }
        catch (InvalidOperationException)
        {
            return StatusCode(StatusCodes.Status502BadGateway,
                new { error = "The AI service returned an invalid response." });
        }
    }
}

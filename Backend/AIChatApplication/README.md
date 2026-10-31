# AIChatApplication

A small layered .NET 10 API for chatting with a local Ollama model.

## Projects

- `AIChatApplication.Api`: HTTP endpoints and API request contracts.
- `AIChatApplication.Application`: chat flow and interfaces used by the application.
- `AIChatApplication.Domain`: chat message and conversation state.
- `AIChatApplication.Infrastructure`: Ollama HTTP integration and in-memory conversation storage.

## Run locally

1. Ensure the .NET 10 SDK is installed.
2. Ensure Ollama is running and the configured model is available (`ollama list`).
3. From this directory, run:

   ```powershell
   dotnet restore .\AIChatApplication.sln
   dotnet build .\AIChatApplication.sln
   dotnet run --project .\src\AIChatApplication.Api\AIChatApplication.Api.csproj
   ```

4. Send a `POST` request to `/api/Chat` with JSON such as:

   ```json
   { "message": "Explain dependency injection", "conversationId": null }
   ```

## Notes

Conversation history is in memory and is lost when the process restarts. The semaphore serializes requests for a conversation within this application process. This sample does not yet authenticate users or enforce conversation ownership, so do not expose it publicly as-is.

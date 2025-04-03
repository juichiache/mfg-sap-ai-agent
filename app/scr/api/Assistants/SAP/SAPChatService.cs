using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using Assistants.API.Core;
using Assistants.API.Services.Prompts;
using Azure.AI.Projects;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using MinimalApi.Services.Search;
using MinimalApi.Services.Search.IndexDefinitions;

namespace Assistants.Hub.API.Assistants.RAG;

public class SAPChatService
{
    private readonly ILogger<SAPChatService> _logger;
    private readonly IConfiguration _configuration;
    private readonly OpenAIClientFacade _openAIClientFacade;

    public SAPChatService(OpenAIClientFacade openAIClientFacade,
                                 ILogger<SAPChatService> logger,
                                 IConfiguration configuration)
    {
        _openAIClientFacade = openAIClientFacade;
        _logger = logger;
        _configuration = configuration;
    }

    public async IAsyncEnumerable<ChatChunkResponse> ExecuteAsync(ChatTurn[] chatMessages, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var chatHistory = new ChatHistory();
        foreach (var turn in chatMessages)
        {
            chatHistory.AddUserMessage(turn.User);
            if (!string.IsNullOrEmpty(turn.Assistant))
                chatHistory.AddAssistantMessage(turn.Assistant);
        }

        await foreach (var chunk in ExecuteAsync(chatHistory, cancellationToken))
        {
            yield return chunk;
        }
    }

    public async IAsyncEnumerable<ChatChunkResponse> ExecuteAsync(ChatHistory chatHistory, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var sw = Stopwatch.StartNew();

        // Kernel setup
        var kernel = _openAIClientFacade.BuildKernel("SAP");
        var chatGpt = kernel.Services.GetService<IChatCompletionService>();
        ArgumentNullException.ThrowIfNull(chatGpt, nameof(chatGpt));
        kernel.Data["VectorSearchSettings"] = new VectorSearchSettings("steel-policies-vectors", 10, AISearchIndexerIndexDefinintion.EmbeddingsFieldName, "text-embedding", 12000, 5, false, false, "", "", false);  

        // Build Chat History
        var systemPrompt = PromptService.GetPromptByName("SAPAgentSystemPrompt");
        chatHistory.AddSystemMessage(systemPrompt);

        // Execute Chat Completion
        var executionSettings = new OpenAIPromptExecutionSettings { FunctionChoiceBehavior = FunctionChoiceBehavior.Auto() };
        var sb = new StringBuilder();
        await foreach (StreamingChatMessageContent responseChunk in chatGpt.GetStreamingChatMessageContentsAsync(chatHistory, executionSettings, kernel, cancellationToken))
        {
            if (responseChunk.Content != null)
            {
                sb.Append(responseChunk.Content);
                yield return new ChatChunkResponse(responseChunk.Content);
                await Task.Yield();
            }
        }
        sw.Stop();

        var thoughtProcess = kernel.GetThoughtProcess(systemPrompt, sb.ToString()).ToList();
        yield return new ChatChunkResponse(string.Empty, new ChatChunkResponseResult(sb.ToString(), thoughtProcess));
    }
}
using Assistants.API.Core;
using Assistants.Hub.API.Assistants.RAG;
using Assistants.Hub.API.Assistants.SAP;
using Azure.Identity;
using Microsoft.Agents.Builder;
using Microsoft.Agents.Hosting.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.SemanticKernel.Agents.AzureAI;
using System.Runtime.CompilerServices;

namespace Assistants.API
{
    internal static class WebApplicationExtensions
    {
        internal static WebApplication MapApi(this WebApplication app)
        {
            var api = app.MapGroup("api");
            api.MapPost("chat/rag/{agentName}", ProcessRagRequest);


            api.MapPost("chat/sap", ProcessSAPRequest);
            api.MapPost("chat/agent/sap/create", ProcessSAPAgentCreate);
            api.MapPost("chat/agent/sap", ProcessSAPAzureAIAgentRequest);

            api.MapPost("messages", ProcessAgentRequest);

            api.MapGet("status", ProcessStatusGet);

            api.MapGet("image/{fileName}", ProcessImageGet);
            return app;
        }


        private static async IAsyncEnumerable<ChatChunkResponse> ProcessRagRequest(string agentName, ChatTurn[] request, [FromServices] RAGChatService aiService, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            await foreach (var chunk in aiService.ReplyPlannerAsync(agentName, request).WithCancellation(cancellationToken))
            {
                yield return chunk;
            }
        }


        private static async IAsyncEnumerable<ChatChunkResponse> ProcessSAPRequest(ChatTurn[] request, [FromServices] SAPChatService aiService, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            await foreach (var chunk in aiService.ExecuteAsync(request, message => Console.Write(message)).WithCancellation(cancellationToken))
            {
                yield return chunk;
            }
        }

        private static async Task<IResult> ProcessSAPAgentCreate([FromServices] SAPAgentBuilder aiService)
        {
            var agent = await aiService.CreateAgentIfNotExistsAsync();
            return Results.Ok(agent.Id);
        }

        private static async IAsyncEnumerable<ChatChunkResponse> ProcessSAPAzureAIAgentRequest(ChatThreadRequest request, [FromServices] SAPAzureAIAgent aiService, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var handler = new Action<string>(x =>
            {
                Console.WriteLine(x);
            });

            await foreach (var chunk in aiService.ExecuteAsync(request, handler).WithCancellation(cancellationToken))
            {
                yield return chunk;
            }
        }

        private static async Task<IResult> ProcessStatusGet()
        {
            return Results.Ok("OK");
        }
        private static async Task<IResult> ProcessImageGet(string fileName, IConfiguration configuration)
        {   
            #pragma warning disable SKEXP0110 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
            var agentsClient = AzureAIAgent.CreateAgentsClient(configuration["AIAgentServiceProjectConnectionString"], new DefaultAzureCredential(new DefaultAzureCredentialOptions { VisualStudioTenantId = configuration["VisualStudioTenantId"] }));
            #pragma warning restore SKEXP0110 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

            var fileContent = await agentsClient.Files.GetFileContentAsync(fileName.Split('.')[0]);


            // Set correct content type
            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(fileName, out var contentType))
            {
                contentType = "application/octet-stream";
            }

            return Results.Stream(fileContent.Value.ToStream(), contentType);
        }

        private static async Task ProcessAgentRequest(HttpRequest request, HttpResponse response,[FromServices]IAgentHttpAdapter adapter, IAgent bot, CancellationToken cancellationToken)
        {
            await adapter.ProcessAsync(request, response, bot, cancellationToken);            
        }
    }
}
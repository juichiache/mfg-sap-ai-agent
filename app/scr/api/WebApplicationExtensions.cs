using Assistants.API.Core;
using Assistants.Hub.API;
using Assistants.Hub.API.Assistants;
using Assistants.Hub.API.Assistants.RAG;
using Assistants.Hub.API.Assistants.SAP;
using Microsoft.Agents.Builder;
using Microsoft.Agents.Hosting.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel.Services;
using MinimalApi.Services;
using System;
using System.Runtime.CompilerServices;

namespace Assistants.API
{
    internal static class WebApplicationExtensions
    {
        internal static WebApplication MapApi(this WebApplication app)
        {
            var api = app.MapGroup("api");
            api.MapPost("chat/weather", ProcessWeatherRequest);
            api.MapPost("chat/autobodydamageanalysis", ProcessAutoDamageAnalysis);
            api.MapPost("chat/rag/{agentName}", ProcessRagRequest);

            api.MapPost("chat/agent", ProcessAgentRequestV2);

            api.MapPost("chat/sap", ProcessSAPRequest);
            api.MapPost("chat/agent/sap/create", ProcessSAPAgentCreate);
            api.MapPost("chat/agent/sap", ProcessSAPAzureAIAgentRequest);

            api.MapPost("messages", ProcessAgentRequest);

            api.MapGet("status", ProcessStatusGet);
            return app;
        }
        private static async IAsyncEnumerable<ChatChunkResponse> ProcessWeatherRequest(ChatTurn[] request, [FromServices] WeatherChatService weatherChatService, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            await foreach (var chunk in weatherChatService.ReplyPlannerAsync(request).WithCancellation(cancellationToken))
            {
                yield return chunk;
            }
        }

        private static async IAsyncEnumerable<ChatChunkResponse> ProcessAutoDamageAnalysis (ChatTurn[] request, [FromServices] AutoDamageAnalysisChatService autoDamageAnalysisChatService, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            await foreach (var chunk in autoDamageAnalysisChatService.ReplyPlannerAsync(request).WithCancellation(cancellationToken))
            {
                yield return chunk;
            }
        }

        private static async IAsyncEnumerable<ChatChunkResponse> ProcessRagRequest(string agentName, ChatTurn[] request, [FromServices] RAGChatService aiService, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            await foreach (var chunk in aiService.ReplyPlannerAsync(agentName, request).WithCancellation(cancellationToken))
            {
                yield return chunk;
            }
        }
        private static async IAsyncEnumerable<ChatChunkResponse> ProcessAgentRequestV2(ChatTurn[] request, [FromServices] AutoAdvisorAgent agent, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            await foreach (var chunk in agent.Execute(request).WithCancellation(cancellationToken))
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

        private static async Task ProcessAgentRequest(HttpRequest request, HttpResponse response,[FromServices]IAgentHttpAdapter adapter, IAgent bot, CancellationToken cancellationToken)
        {
            await adapter.ProcessAsync(request, response, bot, cancellationToken);            
        }
    }
}
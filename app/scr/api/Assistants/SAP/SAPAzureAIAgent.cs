﻿using Assistants.API.Core;
using Azure.AI.Agents.Persistent;
using Azure.Identity;

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Agents.AzureAI;
using Microsoft.SemanticKernel.ChatCompletion;
using MinimalApi.Services.Search;
using MinimalApi.Services.Search.IndexDefinitions;
using System.Runtime.CompilerServices;
using System.Text;

namespace Assistants.Hub.API.Assistants.SAP
{
#pragma warning disable SKEXP0110
    public class SAPAzureAIAgent
    {
        private readonly PersistentAgentsClient _agentsClient;
        private readonly OpenAIClientFacade _openAIClientFacade;
        private readonly IConfiguration _configuration;
        private readonly SAPAgentBuilder _sapAgentBuilder;

        public SAPAzureAIAgent(OpenAIClientFacade openAIClientFacade, IConfiguration configuration, SAPAgentBuilder sapAgentBuilder)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            var aiAgentEndpoint = _configuration["AIAgentEndpoint"];
            ArgumentNullException.ThrowIfNullOrEmpty(aiAgentEndpoint, "AIAgentEndpoint");

            _agentsClient = AzureAIAgent.CreateAgentsClient(aiAgentEndpoint, new DefaultAzureCredential(new DefaultAzureCredentialOptions { VisualStudioTenantId = _configuration["VisualStudioTenantId"] }));
            _openAIClientFacade = openAIClientFacade ?? throw new ArgumentNullException(nameof(openAIClientFacade));
            _sapAgentBuilder = sapAgentBuilder ?? throw new ArgumentNullException(nameof(sapAgentBuilder));
        }

        public async IAsyncEnumerable<ChatChunkResponse> ExecuteAsync(ChatThreadRequest request, Action<string> OnMessageReceived, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var definition = await _sapAgentBuilder.CreateAgentIfNotExistsAsync();
            var kernel = _openAIClientFacade.BuildKernel("SAP");
            var agent = new AzureAIAgent(definition.Definition, _agentsClient, kernel.Plugins);
            agent.Kernel.Data.Add("ChatCompletionsKernel", kernel);
            agent.Kernel.Data.Add("IntermediateMessageHandler", OnMessageReceived);
            agent.Kernel.Data["VectorSearchSettings"] = new VectorSearchSettings("steel-policies-vectors", 10, AISearchIndexerIndexDefinintion.EmbeddingsFieldName, "text-embedding", 12000, 5, false, false, "", "", false);

            var sb = new StringBuilder();
            var userMessage = request.Message;

            var agentThread = new AzureAIAgentThread(agent.Client);
            if (!string.IsNullOrEmpty(request.ThreadId))
                agentThread = new AzureAIAgentThread(agent.Client, request.ThreadId);


            Task ProcessIntermediateMessage(ChatMessageContent message)
            {
                var fccList = message.Items.OfType<FunctionCallContent>().ToList();
                if (OnMessageReceived != null)
                {
                    foreach (var fcc in fccList)
                    {
                        //OnMessageReceived(fcc.FunctionName);
                    }
                }

                return Task.CompletedTask;
            }

            var files = new List<string>();
            var message = new ChatMessageContent(AuthorRole.User, userMessage);
            await foreach (StreamingChatMessageContent contentChunk in agent.InvokeStreamingAsync(message, agentThread, new AgentInvokeOptions() { OnIntermediateMessage = ProcessIntermediateMessage, Kernel = agent.Kernel }))
            {

                if (string.IsNullOrEmpty(contentChunk.Content))
                {
                    var types = contentChunk.Items.Select(x => x.GetType()).ToList();
                    StreamingFunctionCallUpdateContent? functionCall = contentChunk.Items.OfType<StreamingFunctionCallUpdateContent>().SingleOrDefault();
                    if (functionCall != null)
                    {
                        Console.WriteLine($"# FUNCTION CALL - {functionCall.Name}");
                    }
                }

                // Differentiate between assistant and tool messages
                if (contentChunk.Metadata?.ContainsKey(AzureAIAgent.CodeInterpreterMetadataKey) ?? false)
                {
                    yield return new ChatChunkResponse(ChatChunkContentType.Code, contentChunk.Content);
                    await Task.Yield();
                    continue;
                }

                if (contentChunk.Items.OfType<StreamingFileReferenceContent>().Any())
                {
                    var file = contentChunk.Items.OfType<StreamingFileReferenceContent>().FirstOrDefault();
                    if (files.Contains(file.FileId))
                        continue;

                    files.Add(file.FileId);
                    //var fileContent = await agent.Client.GetFileContentAsync(file.FileId);
                    //byte[] bytes = fileContent.Value.ToArray();
                    //string base64 = Convert.ToBase64String(bytes);
                    //var dataUrl = $"data:{"image/png"};base64,{base64}";

                    yield return new ChatChunkResponse(ChatChunkContentType.Image, $"{_configuration["ImageServerBasePath"]}/{file.FileId}.png");
                    await Task.Yield();
                    continue;
                }

                if (!string.IsNullOrEmpty(contentChunk.Content))
                {
                    sb.Append(contentChunk.Content);
                    yield return new ChatChunkResponse(ChatChunkContentType.Text, contentChunk.Content);
                    await Task.Yield();
                }
            }

            //var thoughtProcess = _agent.Kernel.GetThoughtProcess(_agent.Instructions, sb.ToString()).ToList();
            //yield return new ChatChunkResponse(string.Empty, new ChatChunkResponseResult(sb.ToString(), thoughtProcess, agentThread.Id));

            //TODO: Delete agent

        }
    }

#pragma warning disable SKEXP0110
}

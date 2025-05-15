using Assistants.API.Core;
using Assistants.Hub.API.Core;
using Azure.AI.Projects;
using Azure.Identity;

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Agents.AzureAI;
using Microsoft.SemanticKernel.ChatCompletion;
using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace Assistants.Hub.API.Assistants.SAP
{
#pragma warning disable SKEXP0110
    public class SAPAzureAIAgent
    {
        private readonly AzureAIAgent _agent;
        private readonly IConfiguration _configuration;

        public SAPAzureAIAgent(OpenAIClientFacade openAIClientFacade, IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            var azureProjectConnectionString = _configuration["AIAgentServiceProjectConnectionString"];
            var azureAIAgentID = _configuration["AzureAIAgentID"];
            ArgumentNullException.ThrowIfNullOrEmpty(azureProjectConnectionString, "AzureProjectConnectionString");
            ArgumentNullException.ThrowIfNullOrEmpty(azureAIAgentID, "azureAIAgentID");

            AIProjectClient client = AzureAIAgent.CreateAzureAIClient(azureProjectConnectionString, new DefaultAzureCredential(new DefaultAzureCredentialOptions { VisualStudioTenantId = _configuration["VisualStudioTenantId"] }));
            AgentsClient agentsClient = client.GetAgentsClient();
            var definition = agentsClient.GetAgent(azureAIAgentID);

            var kernel = openAIClientFacade.BuildKernel("SAP");

            var p = kernel.Plugins;
            var agent = new AzureAIAgent(definition, agentsClient, p);
            agent.Kernel.Data.Add("ChatCompletionsKernel", kernel);

            _agent = agent;
        }

        public async IAsyncEnumerable<ChatChunkResponse> ExecuteAsync(ChatThreadRequest request, Action<string> OnMessageReceived, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var sb = new StringBuilder();
            var userMessage = request.Message;

            var agentThread = new AzureAIAgentThread(_agent.Client);
            if (!string.IsNullOrEmpty(request.ThreadId))
                agentThread = new AzureAIAgentThread(_agent.Client, request.ThreadId);


            Task OnNewMessage(ChatMessageContent message)
            {
                if(OnMessageReceived != null)
                {
                    OnMessageReceived(message.Content);
                }
                else
                {
                    Console.WriteLine(message.Content);
                }

                return Task.CompletedTask;
            }

            var message = new ChatMessageContent(AuthorRole.User, userMessage);
            await foreach (StreamingChatMessageContent contentChunk in _agent.InvokeStreamingAsync(message, agentThread, new AgentInvokeOptions() { OnIntermediateMessage = OnNewMessage }))
            {

                if (string.IsNullOrEmpty(contentChunk.Content))
                {
                    StreamingFunctionCallUpdateContent? functionCall = contentChunk.Items.OfType<StreamingFunctionCallUpdateContent>().SingleOrDefault();
                    if (functionCall != null)
                    {
                      
                        if (OnMessageReceived != null)
                        {
                            OnMessageReceived(message.Content);
                        }
                        else
                        {
                            Console.WriteLine($"# FUNCTION CALL - {functionCall.Name}");
                        }
                    }

                    continue;
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
                    yield return new ChatChunkResponse(ChatChunkContentType.Image, file.FileId);
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
        }

        public IEnumerable<ExecutionStepResult> GetExecutionSteps()
        {
            var steps = _agent.Kernel.GetFunctionCallResults().ToList();
            return steps;
        }

        public bool ContainsStreamingFileReferenceContent(IEnumerable<object> contentItems)
        {
            // Check if any item in the collection is of type StreamingFileReferenceContent
            return contentItems.OfType<StreamingFileReferenceContent>().Any();
        }
    }

#pragma warning disable SKEXP0110
}

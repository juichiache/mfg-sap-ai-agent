using Assistants.API.Core;
using Azure.AI.Projects;
using Azure.Identity;

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Agents.AzureAI;
using Microsoft.SemanticKernel.ChatCompletion;

using System.Runtime.CompilerServices;
using System.Text;

namespace Assistants.Hub.API.Assistants.SAP
{
#pragma warning disable SKEXP0110
    public class SAPAzureAIAgent
    {
        private readonly AgentsClient _agentsClient;
        private readonly OpenAIClientFacade _openAIClientFacade;
        private readonly IConfiguration _configuration;

        public SAPAzureAIAgent(OpenAIClientFacade openAIClientFacade, IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            var azureProjectConnectionString = _configuration["AIAgentServiceProjectConnectionString"];
            var azureAIAgentID = _configuration["AzureAIAgentID"];
            ArgumentNullException.ThrowIfNullOrEmpty(azureProjectConnectionString, "AzureProjectConnectionString");
            ArgumentNullException.ThrowIfNullOrEmpty(azureAIAgentID, "azureAIAgentID");

            AIProjectClient client = AzureAIAgent.CreateAzureAIClient(azureProjectConnectionString, new DefaultAzureCredential(new DefaultAzureCredentialOptions { VisualStudioTenantId = _configuration["VisualStudioTenantId"] }));
            _agentsClient = client.GetAgentsClient();
            _openAIClientFacade = openAIClientFacade ?? throw new ArgumentNullException(nameof(openAIClientFacade));
        }

        public async IAsyncEnumerable<ChatChunkResponse> ExecuteAsync(ChatThreadRequest request, Action<string> OnMessageReceived, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var definition = _agentsClient.GetAgent(_configuration["AzureAIAgentID"]);
            var kernel = _openAIClientFacade.BuildKernel("SAP");
            var agent = new AzureAIAgent(definition, _agentsClient, kernel.Plugins);
            agent.Kernel.Data.Add("ChatCompletionsKernel", kernel);
            agent.Kernel.Data.Add("IntermediateMessageHandler", OnMessageReceived);


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

            
            var message = new ChatMessageContent(AuthorRole.User, userMessage);
            await foreach (StreamingChatMessageContent contentChunk in agent.InvokeStreamingAsync(message, agentThread, new AgentInvokeOptions() { OnIntermediateMessage = ProcessIntermediateMessage, Kernel = agent.Kernel }))
            {

                if (string.IsNullOrEmpty(contentChunk.Content))
                {
                    var types = contentChunk.Items.Select(x => x.GetType()).ToList();
                    StreamingFunctionCallUpdateContent ? functionCall = contentChunk.Items.OfType<StreamingFunctionCallUpdateContent>().SingleOrDefault();
                    if (functionCall != null)
                    {
                        Console.WriteLine($"# FUNCTION CALL - {functionCall.Name}");
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
                    var fileContent = await _agent.Client.GetFileContentAsync(file.FileId);
                    byte[] bytes = fileContent.Value.ToArray();
                    string base64 = Convert.ToBase64String(bytes);
                    var dataUrl = $"data:{"image/png"};base64,{base64}";

                    yield return new ChatChunkResponse(ChatChunkContentType.Image, dataUrl);
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
    }

#pragma warning disable SKEXP0110
}

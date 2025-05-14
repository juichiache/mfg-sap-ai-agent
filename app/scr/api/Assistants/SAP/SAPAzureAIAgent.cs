using Assistants.API.Core;
using Assistants.Hub.API.Core;
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



        public async IAsyncEnumerable<ChatChunkResponse> ExecuteAsync(ChatTurn[] chatMessages, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var sb = new StringBuilder();
            var userMessage = chatMessages.LastOrDefault().User;

            var agentThread = new AzureAIAgentThread(_agent.Client);
            var message = new ChatMessageContent(AuthorRole.User, userMessage);

            await foreach (StreamingChatMessageContent contentChunk in _agent.InvokeStreamingAsync(message, agentThread))
            {
                sb.Append(contentChunk.Content);
                yield return new ChatChunkResponse(contentChunk.Content);
                await Task.Yield();
            }

            var thoughtProcess = _agent.Kernel.GetThoughtProcess(_agent.Instructions, sb.ToString()).ToList();
            yield return new ChatChunkResponse(string.Empty, new ChatChunkResponseResult(sb.ToString(), thoughtProcess));
        }

        public IEnumerable<ExecutionStepResult> GetExecutionSteps()
        {
            var steps = _agent.Kernel.GetFunctionCallResults().ToList();
            return steps;
        }
    }

#pragma warning disable SKEXP0110
}

using Assistants.API.Core;
using Azure.AI.Agents.Persistent;
using Azure.Identity;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents.AzureAI;
using System.Configuration;
using System.Reflection;

namespace Assistants.Hub.API.Assistants.SAP
{
#pragma warning disable SKEXP0110
    public class SAPAgentBuilder
    {

        private readonly Kernel _kernel;
        private readonly IConfiguration _configuration;


        public SAPAgentBuilder(OpenAIClientFacade openAIClientFacade, IConfiguration configuration)
        {
            _configuration = configuration;
            _kernel = openAIClientFacade.BuildKernel("SAP");
        }

        public async Task<AzureAIAgent> CreateAgentIfNotExistsAsync()
        {
            var agentsClient = AzureAIAgent.CreateAgentsClient(_configuration["AIAgentEndpoint"], new DefaultAzureCredential(new DefaultAzureCredentialOptions { VisualStudioTenantId = _configuration["VisualStudioTenantId"]}));

            var tools = new List<FunctionToolDefinition>();
            foreach (var plugin in _kernel.Plugins)
            {
                var pluginTools = plugin.Select(f => f.ToToolDefinition(plugin.Name));
                tools.AddRange(pluginTools);
            }

            var codeInterpreterToolResource = new CodeInterpreterToolResource();

            var definition = await agentsClient.Administration.CreateAgentAsync(
                _configuration["AOAIStandardChatGptDeployment"],//"gpt-4o",
                name: "mfg-sap-agent",
                instructions: LoadEmbeddedResource("Assistants.Hub.API.Services.Prompts.SAPAgentSystemPrompt.txt"),
                tools: new List<ToolDefinition>() { new CodeInterpreterToolDefinition() },
                toolResources: new ToolResources() { CodeInterpreter = codeInterpreterToolResource });

            AzureAIAgent agent = new(definition, agentsClient, plugins: _kernel.Plugins);

            return agent;
        }

        private string LoadEmbeddedResource(string resourceName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            using var stream = assembly.GetManifestResourceStream(resourceName);
            using var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }
    }

#pragma warning disable SKEXP0110
}

using Assistants.API.Core;
using Azure.AI.Projects;
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
            AIProjectClient client = AzureAIAgent.CreateAzureAIClient(_configuration["AIAgentServiceProjectConnectionString"], new DefaultAzureCredential(new DefaultAzureCredentialOptions { VisualStudioTenantId = _configuration["VisualStudioTenantId"]}));
            AgentsClient agentsClient = client.GetAgentsClient();

            var tools = new List<FunctionToolDefinition>();
            foreach (var plugin in _kernel.Plugins)
            {
                var pluginTools = plugin.Select(f => f.ToToolDefinition(plugin.Name));
                tools.AddRange(pluginTools);
            }

            Azure.AI.Projects.Agent definition = await agentsClient.CreateAgentAsync(
                "gpt-4o",
                name: "rutzsco-sap-agent",
                instructions: LoadEmbeddedResource("Assistants.Hub.API.Services.Prompts.SAPAgentSystemPrompt.txt"));

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

using Assistants.Hub.API.Assistants.RAG;
using Assistants.Hub.API.Assistants.SAP;
using Azure;
using Microsoft.Agents.Builder;
using Microsoft.Agents.Hosting.AspNetCore;
using Microsoft.Agents.Storage;
using MinimalApi.Services.Search;


namespace Assistants.API.Core
{
    internal static class ServiceCollectionExtensions
    {
        internal static IServiceCollection AddAzureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpClient("WeatherAPI", client =>
            {
                client.BaseAddress = new Uri("https://api.weather.gov/");
            });
            services.AddHttpClient("SAPDATAAPI", client =>
            {
                client.BaseAddress = new Uri(configuration["SAPDataServiceEndpoint"]);
            });
            var azureSearchServiceKey = configuration["AzureSearchServiceKey"];
            if (!string.IsNullOrEmpty(azureSearchServiceKey))
            {
                services.AddSingleton<SearchClientFactory>(sp =>
                {
                    var config = sp.GetRequiredService<IConfiguration>();
                    return new SearchClientFactory(config, null, new AzureKeyCredential(azureSearchServiceKey));
                });
            }

            services.AddSingleton<OpenAIClientFacade>(sp =>
            {
                var config = sp.GetRequiredService<IConfiguration>();
                var standardChatGptDeployment = config["AOAIStandardChatGptDeployment"];
                var standardServiceEndpoint = config["AOAIStandardServiceEndpoint"];
                var standardServiceKey = config["AOAIStandardServiceKey"];


                var facade =  new OpenAIClientFacade(configuration, new Azure.AzureKeyCredential(standardServiceKey), null, sp.GetRequiredService<IHttpClientFactory>(), sp.GetRequiredService<SearchClientFactory>());
                return facade;
            });

            services.AddSingleton<SAPAzureAIAgent>();
            services.AddSingleton<SAPAgentBuilder>();
            services.AddSingleton<SAPChatService>();
            services.AddSingleton<RAGChatService>();
            return services;
        }


        public static IHostApplicationBuilder AddAgent<TAgent, THandler>(this IHostApplicationBuilder builder) where TAgent : IAgent where THandler : class, TAgent
        {
            builder.Services.AddAgentAspNetAuthentication(builder.Configuration);

            builder.AddAgentApplicationOptions();

            builder.AddAgent<THandler>();

            builder.Services.AddSingleton<IStorage, MemoryStorage>();

            return builder;
        }
    }
}
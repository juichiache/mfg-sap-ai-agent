using Microsoft.SemanticKernel;
using MinimalApi.Services;
using MinimalApi.Services.Skills;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using Microsoft.Agents.Authentication;
using Microsoft.Agents.Hosting.AspNetCore;
using MinimalApi.Services.Search;
using Assistants.Hub.API.Assistants.RAG;
using Azure;
using Assistants.Hub.API.Assistants;
using Assistants.Hub.API.Core;
using Assistants.Hub.API;
using Microsoft.Agents.Storage;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Agents.Builder.App;
using Microsoft.Agents.Builder;

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
            var azureSearchServiceKey = configuration["AzureSearchServiceEndpoint"];
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

            services.AddSingleton<SAPChatService>();
            services.AddSingleton<AutoAdvisorAgent>();
            services.AddSingleton<RAGChatService>();
            services.AddSingleton<WeatherChatService>();
            services.AddSingleton<AutoDamageAnalysisChatService>();
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
using Assistants.API.Core;
using Assistants.API.Services.Prompts;
using Azure.AI.OpenAI;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using MinimalApi.Services.Search;
using MinimalApi.Services.Search.IndexDefinitions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.ComponentModel;

namespace Assistants.Hub.API.Assistants.RAG;

public class SAPRetrivalPlugins
{
    private readonly SearchClientFactory _searchClientFactory;
    private readonly AzureOpenAIClient _azureOpenAIClient;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string _apiKey;

    public SAPRetrivalPlugins(SearchClientFactory searchClientFactory, AzureOpenAIClient azureOpenAIClient, IHttpClientFactory httpClientFactory, string apiKey)
    {
        _httpClientFactory = httpClientFactory;
        _apiKey = apiKey;
        _searchClientFactory = searchClientFactory;
        _azureOpenAIClient = azureOpenAIClient;
    }


    [KernelFunction("get_inbound_deliveries")]
    [Description("Get a list of inbound deliveries")]
    [return: Description("A list of inbound deliveries")]
    public async Task<string> GetInboundDeliveryAsync(Kernel kernel)
    {
        kernel.AddIntermediateMessage("Retrieving inbound deliveries...");
        using var httpClient = _httpClientFactory.CreateClient("SAPDATAAPI");
        httpClient.DefaultRequestHeaders.Add("x-functions-key", _apiKey);

        // Make the API call to get inventory data
        var response = await httpClient.GetAsync($"inbound-deliveries?dateFrom=2025-01-01&dateTo=2025-04-02");
        response.EnsureSuccessStatusCode();

        var responseBody = await response.Content.ReadAsStringAsync();
        return responseBody;
    }

    [KernelFunction("get_inventory")]
    [Description("Get a list of current inventory")]
    [return: Description("A list of inventory items")]
    public async Task<string> GetInventoryAsync(Kernel kernel)
    {
        kernel.AddIntermediateMessage("Retrieving inventory...");

        using var httpClient = _httpClientFactory.CreateClient("SAPDATAAPI");
        httpClient.DefaultRequestHeaders.Add("x-functions-key", _apiKey);

        // Make the API call to get inventory data
        var response = await httpClient.GetAsync("inventory");
        response.EnsureSuccessStatusCode();

        var responseBody = await response.Content.ReadAsStringAsync();
        return responseBody;
    }

    [KernelFunction("get_purchase_orders")]
    [Description("Get a list of current purchase orders")]
    [return: Description("A list of purchase orders")]
    public async Task<string> GetPurchaseOrdersAsync(Kernel kernel)
    {
        kernel.AddIntermediateMessage("Retrieving purchase orders...");

        using var httpClient = _httpClientFactory.CreateClient("SAPDATAAPI");
        httpClient.DefaultRequestHeaders.Add("x-functions-key", _apiKey);

        // Make the API call to get inventory data
        var response = await httpClient.GetAsync("purchase-orders");
        response.EnsureSuccessStatusCode();

        var responseBody = await response.Content.ReadAsStringAsync();
        return responseBody;
    }

    [KernelFunction("get_policy_insights")]
    [Description("Gets relevant tariff proposals government policy information based on the provided search term.")]
    [return: Description("A list relevant relevant tariff proposals government policy information based on the provided search term.")]
    public async Task<IEnumerable<KnowledgeSource>> GetKnowledgeSourcesAsync(Kernel kernel, [Description("Search query")] string searchQuery)
    {
        try
        {
            kernel.AddIntermediateMessage("Retrieving policy insights...");

            var settings = kernel.Data["VectorSearchSettings"] as VectorSearchSettings;
            if (settings == null)
                throw new ArgumentNullException(nameof(settings), "VectorSearchSettings cannot be null");

            var logic = new SearchLogic<AISearchIndexerIndexDefinintion>(_azureOpenAIClient, _searchClientFactory, AISearchIndexerIndexDefinintion.SelectFieldNames, settings);
            var results = await logic.SearchAsync(searchQuery);

            // Add kernel context for diagnostics
            kernel.AddFunctionCallResult("get_policy_insights", $"Search Query: {searchQuery} /n {System.Text.Json.JsonSerializer.Serialize(results)}", results);

            return results;
        }
        catch (Exception ex)
        {
            kernel.AddFunctionCallResult("get_policy_insights", $"Error: {ex.Message}", null);
            throw;
        }
    }

    [KernelFunction("get_weather_forecast")]
    [Description("Get weather forecast for a specified location point")]
    [return: Description("A weather forecast in JSON format")]
    public async Task<string> RetrieveWeatherForecastAsync(Kernel kernel, [Description("Location coordinates")] LocationPoint locationPoint, KernelArguments arguments)
    {
        kernel.AddIntermediateMessage("Retrieving weather forecast...");

        using var httpClient = _httpClientFactory.CreateClient("WeatherAPI");
        httpClient.DefaultRequestHeaders.Add("User-Agent", "app");

        // Get forecast URL
        var response = await httpClient.GetAsync($"points/{locationPoint.Latitude},{locationPoint.Longitude}");
        response.EnsureSuccessStatusCode();

        var responseBody = await response.Content.ReadAsStringAsync();
        var json = JObject.Parse(responseBody);

        if (json["properties"]?["forecast"]?.ToString() is not string forecastUrl)
        {
            throw new InvalidOperationException("Invalid forecast response format");
        }

        // Get forecast data
        var forecastResponse = await httpClient.GetAsync(forecastUrl);
        forecastResponse.EnsureSuccessStatusCode();

        var forecastResponseBody = await forecastResponse.Content.ReadAsStringAsync();
        arguments["WeatherForecast"] = forecastResponseBody;

        return forecastResponseBody;
    }

    [KernelFunction("get_location_lat_long")]
    [Description("Determine latitude and longitude from a location description")]
    [return: Description("Location coordinates with latitude and longitude")]
    public async Task<LocationPoint> DetermineLatLongAsync([Description("Location name or zip code")] string weatherLocation, KernelArguments arguments, Kernel kernel)
    {
        
        var chatCompletionsKernel = kernel;
        if (kernel.Data.ContainsKey("ChatCompletionsKernel"))
        {
            chatCompletionsKernel = kernel.Data["ChatCompletionsKernel"] as Kernel;
        }

        var chatHistory = new ChatHistory(PromptService.GetPromptByName("WeatherLatLongSystemPrompt"));
        chatHistory.AddUserMessage(weatherLocation);

        var chatGpt = chatCompletionsKernel.Services.GetService<IChatCompletionService>();
        var searchAnswer = await chatGpt.GetChatMessageContentAsync(
            chatHistory, DefaultSettings.AISearchRequestSettings, kernel);

        var parts = searchAnswer.Content.Split(',');
        if (parts.Length != 2)
        {
            throw new ArgumentException(
                "Invalid location format. Expected 'latitude, longitude'.");
        }

        var lp = new LocationPoint
        {
            Latitude = parts[0].Trim(),
            Longitude = parts[1].Trim()
        };

        arguments["LocationPoint"] = lp;
        return lp;
    }
}

public class LocationPoint
{
    [JsonProperty("latitude")]
    public string Latitude { get; set; }

    [JsonProperty("longitude")]
    public string Longitude { get; set; }
}
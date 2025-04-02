using System.ComponentModel;
using Assistants.API.Core;
using Assistants.API.Services.Prompts;
using Assistants.Hub.API.Assistants.SAP;
using Azure.AI.OpenAI;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using MinimalApi.Services.Search;
using MinimalApi.Services.Search.IndexDefinitions;
using MinimalApi.Services.Skills;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Assistants.Hub.API.Assistants.RAG;

public class SAPRetrivalPlugins
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string _apiKey;

    public SAPRetrivalPlugins(IHttpClientFactory httpClientFactory, string apiKey)
    {
        _httpClientFactory = httpClientFactory;
        _apiKey = apiKey;
    }


    [KernelFunction("GetInboundDeliveries")]
    [Description("Get a list of inbound deliveries")]
    [return: Description("A list of inbound deliveries")]
    public async Task<string> GetInboundDeliveryAsync()
    {
        using var httpClient = _httpClientFactory.CreateClient("SAPDATAAPI");
        //httpClient.DefaultRequestHeaders.Add("x-functions-key", _apiKey);
        httpClient.DefaultRequestHeaders.Add("accept", "application/json");

        // Make the API call to get inventory data
        var response = await httpClient.GetAsync($"inbound-deliveries?dateFrom=2025-01-01&dateTo=2025-04-02&code={_apiKey}");
        response.EnsureSuccessStatusCode();

        var responseBody = await response.Content.ReadAsStringAsync();
        return responseBody;
    }

    //[Description("GetInventory")]
    //[return: Description("A list of inventory items")]
    //public async Task<List<InventoryItem>> GetInventoryAsync()
    //{
    //    using var httpClient = _httpClientFactory.CreateClient("SAPDATAAPI");
    //    httpClient.DefaultRequestHeaders.Add("KEY", _apiKey);

    //    // Make the API call to get inventory data
    //    var response = await httpClient.GetAsync("/inbound-deliveries");
    //    response.EnsureSuccessStatusCode();

    //    var responseBody = await response.Content.ReadAsStringAsync();
    //    var inventoryItems = JsonConvert.DeserializeObject<List<InventoryItem>>(responseBody);

    //    return inventoryItems;
    //}



    //[Description("GetCommodityPricesAsync ")]
    //[return: Description("A list of inbound deliveries")]
    //public async Task<List<DeliverySummary>> GetCommodityPricesAsync()
    //{
    //    using var httpClient = _httpClientFactory.CreateClient("SAPDATAAPI");
    //    httpClient.DefaultRequestHeaders.Add("User-Agent", "app");

    //    // Make the API call to get inventory data
    //    var response = await httpClient.GetAsync("deliveries");
    //    response.EnsureSuccessStatusCode();

    //    var responseBody = await response.Content.ReadAsStringAsync();
    //    var inventoryItems = JsonConvert.DeserializeObject<List<DeliverySummary>>(responseBody);

    //    return inventoryItems;
    //}

    //[Description("GetTariffPolicyUpdatesAsync  ")]
    //[return: Description("A list of inbound deliveries")]
    //public async Task<List<DeliverySummary>> GetTariffPolicyUpdatesAsync()
    //{
    //    using var httpClient = _httpClientFactory.CreateClient("SAPDATAAPI");
    //    httpClient.DefaultRequestHeaders.Add("User-Agent", "app");

    //    // Make the API call to get inventory data
    //    var response = await httpClient.GetAsync("deliveries");
    //    response.EnsureSuccessStatusCode();

    //    var responseBody = await response.Content.ReadAsStringAsync();
    //    var inventoryItems = JsonConvert.DeserializeObject<List<DeliverySummary>>(responseBody);

    //    return inventoryItems;
    //}

    //[Description("GetPurchaseOrdersAsync ")]
    //[return: Description("A list of purchase orders")]
    //public async Task<List<PurchaseOrder>> GetPurchaseOrdersAsync()
    //{
    //    using var httpClient = _httpClientFactory.CreateClient("SAPDATAAPI");
    //    httpClient.DefaultRequestHeaders.Add("User-Agent", "app");

    //    // Make the API call to get inventory data
    //    var response = await httpClient.GetAsync("po");
    //    response.EnsureSuccessStatusCode();

    //    var responseBody = await response.Content.ReadAsStringAsync();
    //    var inventoryItems = JsonConvert.DeserializeObject<List<PurchaseOrder>>(responseBody);

    //    return inventoryItems;
    //}

    [KernelFunction("GetWeatherForecast")]
    [Description("Get weather forecast for a specified location point")]
    [return: Description("A weather forecast in JSON format")]
    public async Task<string> RetrieveWeatherForecastAsync([Description("Location coordinates")] LocationPoint locationPoint, KernelArguments arguments)
    {
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

    [KernelFunction("GetLocationLatLong")]
    [Description("Determine latitude and longitude from a location description")]
    [return: Description("Location coordinates with latitude and longitude")]
    public async Task<LocationPoint> DetermineLatLongAsync([Description("Location name or zip code")] string weatherLocation, KernelArguments arguments, Kernel kernel)
    {
        var chatGpt = kernel.Services.GetService<IChatCompletionService>();
        var chatHistory = new ChatHistory(PromptService.GetPromptByName("WeatherLatLongSystemPrompt"));
        chatHistory.AddUserMessage(weatherLocation);

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
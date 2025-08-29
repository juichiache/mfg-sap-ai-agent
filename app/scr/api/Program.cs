using Assistants.API;
using Assistants.API.Core;
using Assistants.Hub.API.M365;
using Microsoft.Agents.Builder;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();
}

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAzureServices(builder.Configuration);
builder.AddAgent<IAgent, SapAgent>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(options => options.OpenApiVersion = OpenApiSpecVersion.OpenApi3_0);
    app.UseSwaggerUI();
}

if (app.Environment.IsProduction())
{
    app.UseMiddleware<ApiKeyMiddleware>();
}

app.UseHttpsRedirection();

app.MapApi();
app.Run();

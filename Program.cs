using Microsoft.Azure.Cosmos;
using Services;
using SupportWebApp.Components;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);
var cosmosConnectionString =
builder.Configuration["CosmosDb:ConnectionString"]
?? throw new InvalidOperationException(
"CosmosDb:ConnectionString is missing.");

var databaseName =
builder.Configuration["CosmosDb:DatabaseName"]
?? throw new InvalidOperationException(
"CosmosDb:DatabaseName is missing.");

var containerName =
builder.Configuration["CosmosDb:ContainerName"]
?? throw new InvalidOperationException(
"CosmosDb:ContainerName is missing.");

if (string.IsNullOrWhiteSpace(cosmosConnectionString))
{
    throw new InvalidOperationException(
    "CosmosDb:ConnectionString mangler. " +
    "Tilføj den i appsettings.json, User Secrets " +
    "eller som environment variable.");
}
Console.WriteLine(
$"Connection string fundet: {!string.IsNullOrWhiteSpace(cosmosConnectionString)}");

Console.WriteLine(
$"Starter med AccountEndpoint=: " +
$"{cosmosConnectionString?.StartsWith("AccountEndpoint=")}");

Console.WriteLine(
$"Indeholder AccountKey=: " +
$"{cosmosConnectionString?.Contains("AccountKey=")}");

builder.Services.AddSingleton<CosmosClient>(_ =>
{
    var options = new CosmosClientOptions
    {
        UseSystemTextJsonSerializerWithOptions =
    new JsonSerializerOptions
    {
        //insures serialization do not convert id -> Id (cosmosDB need lower case id)
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    }
    };

    return new CosmosClient(
    cosmosConnectionString,
    options);
});
builder.Services.AddSingleton<ICosmosDbService>(serviceProvider =>
{
    var cosmosClient =
    serviceProvider.GetRequiredService<CosmosClient>();

    return new CosmosDbService(
    cosmosClient,
    databaseName,
    containerName);
});

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

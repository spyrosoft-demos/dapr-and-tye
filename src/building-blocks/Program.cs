using Dapr.Client;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDaprClient(options =>
{
    var jsonSerializerOptions = new JsonSerializerOptions();
    jsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    jsonSerializerOptions.PropertyNameCaseInsensitive = true;
    options.UseJsonSerializationOptions(jsonSerializerOptions);
});

var app = builder.Build();

app.UseCloudEvents();
app.MapSubscribeHandler();

//app.UseHttpsRedirection();


// create client
var daprClient = app.Services.GetRequiredService<DaprClient>();

//-------------------------------
//  secrets building block
//-------------------------------
var secret = await daprClient.GetSecretAsync("local-secret-store", "twitter-secret");
Console.WriteLine("secret keys: " + string.Join(",", secret.Keys));


//-------------------------------
//  state building block
//-------------------------------
const string stateJson = """
    "Root": {
        "key1": "abc",
        "key2": "123"
    }
    """;
await daprClient.SaveStateAsync("local-state-store", "mystate", stateJson);
var state = await daprClient.GetStateAsync<string>("local-state-store", "mystate");
Console.WriteLine("loaded state: " + state);


//-------------------------------
//  configuration building block
//-------------------------------
var keys = new List<string>
{
    "proxy-setttings",
    "pro"
};
var cfg = await daprClient.GetConfiguration("local-config-store", new List<string>());


//-------------------------------
//  input binding building block
//-------------------------------
app.MapPost("/notifications", (ILoggerFactory loggerFactory) => {
    var log = loggerFactory.CreateLogger("Cron");
    //var msg = FormattableString.Invariant($"cron notification received: {DateTime.UtcNow:dddd, dd MMMM yyyy HH:mm:ss}");
    log.LogInformation("cron notification received: {TimeStamp:dddd, dd MMMM yyyy HH:mm:ss}", DateTime.UtcNow);    
    return Results.Ok();
});


//-------------------------------
// pub sub
//-------------------------------



app.MapGet("/api/health", () => FormattableString.Invariant($"{DateTime.UtcNow:dddd, dd MMMM yyyy HH:mm:ss}"));
app.Run();

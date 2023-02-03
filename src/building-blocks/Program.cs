using Dapr.Client;
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

//  secrets building block
var secret = await daprClient.GetSecretAsync("local-secret-store", "twitter-secret");
Console.WriteLine(secret);

//  state building block
const string json = """
    "Root": {
        "key1": "abc",
        "key2": "123"
    }
    """;
await daprClient.SaveStateAsync("local-state-store", "mystate", json);
var state = await daprClient.GetStateAsync<string>("local-state-store", "mystate");
Console.WriteLine(state);

//  configuration building block
var keys = new List<string>
{
    "proxy-setttings",
    "pro"
};
var cfg = await daprClient.GetConfiguration("local-config-store", new List<string>());

//  bindings
//  TODO: 

// pub sub
//  TODO: 


app.MapGet("/api/health", () => FormattableString.Invariant($"{DateTime.UtcNow:dddd, dd MMMM yyyy HH:mm:ss}"));
app.Run();

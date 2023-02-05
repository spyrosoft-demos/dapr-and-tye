using Dapr.Client;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

//-------------------------------
//  register Dapr client with DI
//-------------------------------
builder.Services.AddDaprClient(options =>
{
    var jsonSerializerOptions = new JsonSerializerOptions();
    jsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    jsonSerializerOptions.PropertyNameCaseInsensitive = true;
    options.UseJsonSerializationOptions(jsonSerializerOptions);
});



var app = builder.Build();
//app.UseHttpsRedirection(); <-- IMPORTANT!



//-------------------------------
//  add needed middlewares 
//-------------------------------
app.UseCloudEvents();
app.MapSubscribeHandler();



//-------------------------------
// create Dapr client
//-------------------------------
var daprClient = app.Services.GetRequiredService<DaprClient>();



//-------------------------------
//  secrets building block
//-------------------------------
var secret = await daprClient.GetSecretAsync(Constants.SecretComponentName, "myapp-secret");
Console.WriteLine("secret keys: " + string.Join(", ", secret.Keys));
Console.WriteLine("secret values: " + string.Join(", ", secret.Values));



//-------------------------------
//  state building block
//-------------------------------
await daprClient.SaveStateAsync(Constants.StateComponentName, "mystate", Constants.SampleState);
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
var cfg = await daprClient.GetConfiguration(Constants.ConfigComponentName, new List<string>());



//-------------------------------
// pub sub building block
//-------------------------------
app.MapPost("pubsub-example", (TopicMessage msg, ILoggerFactory loggerFactory) =>
{
    var log = loggerFactory.CreateLogger("BuildingBlocks.PubSub.MyPubSub");
    log.LogInformation("topic message received: {@Message} {TimeStamp:dddd, dd MMMM yyyy HH:mm:ss}", msg, DateTime.UtcNow);
    return Results.Ok();
}).WithTopic(Constants.PubsubComponentName, Constants.TopicName);



//-------------------------------
//  input binding building block
//-------------------------------
app.MapPost("notifications", async (ILoggerFactory loggerFactory) =>
{
    var log = loggerFactory.CreateLogger("BuildingBlocks.InputBinding.Cron");
    log.LogInformation("cron notification received: {TimeStamp:dddd, dd MMMM yyyy HH:mm:ss}", DateTime.UtcNow);

    //  pubsub publis to topic example
    await daprClient.PublishEventAsync(Constants.PubsubComponentName, Constants.TopicName, new TopicMessage { Id = Guid.NewGuid(), Data = "Some dummy string payload" });
    return Results.Ok();
});

app.MapGet("/api/health", () => FormattableString.Invariant($"{DateTime.UtcNow:dddd, dd MMMM yyyy HH:mm:ss}"));
app.Run();

class TopicMessage
{
    public Guid Id { get; set; }
    public string? Data { get; set; }
}

class Constants
{
    public const string SecretComponentName = "local-secret-store";
    public const string StateComponentName = "local-state-store";
    public const string ConfigComponentName = "local-config-store";
    public const string PubsubComponentName = "my-pubsub";
    public const string TopicName = "demo-topic";
    public const string SampleState = """
    "Root": {
        "key1": "abc",
        "key2": "123"
    }
    """;
}
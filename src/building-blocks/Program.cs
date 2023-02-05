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

//-------------------------------
//  secrets building block
//-------------------------------
var secret = await daprClient.GetSecretAsync("local-secret-store", "myapp-secret");
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
// pub sub
//-------------------------------
app.MapPost("pubsub-example", (TopicMessage msg, ILoggerFactory loggerFactory) =>
{
    var log = loggerFactory.CreateLogger("BuildingBlocks.PubSub.MyPubSub");
    log.LogInformation("topic message received: {@Message} {TimeStamp:dddd, dd MMMM yyyy HH:mm:ss}", msg, DateTime.UtcNow);
    return Results.Ok();
}).WithTopic(TopicMessage.PubsubName, TopicMessage.TopicName);


//-------------------------------
//  input binding building block
//-------------------------------
app.MapPost("/notifications", async (ILoggerFactory loggerFactory) =>
{
    var log = loggerFactory.CreateLogger("BuildingBlocks.InputBinding.Cron");
    log.LogInformation("cron notification received: {TimeStamp:dddd, dd MMMM yyyy HH:mm:ss}", DateTime.UtcNow);
    
    //  just to show pubsub publishing
    await daprClient.PublishEventAsync(TopicMessage.PubsubName, TopicMessage.TopicName, new TopicMessage { Id = Guid.NewGuid(), Data = "Some dummy string payload" });
    return Results.Ok();
});

app.MapGet("/api/health", () => FormattableString.Invariant($"{DateTime.UtcNow:dddd, dd MMMM yyyy HH:mm:ss}"));
app.Run();

class TopicMessage
{
    public const string PubsubName = "my-pubsub";
    public const string TopicName = "demo-topic";


    public Guid Id { get; set; }
    public string? Data { get; set; }
}

using Dapr.Client;
using SpyrosoftLearn.Services.Interfaces;

namespace SpyrosoftLearn.Services;

public class LuckyNumberService : ILuckyNumberService
{
    private readonly DaprClient _daprClient;
    
    public LuckyNumberService(DaprClient daprClient)
    {
        _daprClient = daprClient;
    }

    public async Task<int> GetWinnerNumber(int roundId, int minNumber, int maxNumber)
    {
        //  Dapr - Service to service
        var winnerNumber = await _daprClient.InvokeMethodAsync<int>(
            HttpMethod.Get,
            "luckynumberservice",
            "/api/LuckyNumber/GetWinnerNumber/"+roundId + "/" + minNumber + "/" + maxNumber
            );

        return winnerNumber;
    }

    public async Task PublishWinner(string winnerName)
    {
        //  Dapr - Pub/Sub
        await _daprClient.PublishEventAsync("my-pubsub", "newWinner", winnerName);
    }
}

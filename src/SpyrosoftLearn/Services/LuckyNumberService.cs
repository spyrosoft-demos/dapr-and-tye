﻿using Dapr.Client;

namespace SpyrosoftLearn.Services
{
    public class LuckyNumberService : ILuckyNumberService
    {
        private readonly DaprClient _daprClient;
        private static readonly string storeName = "statestore";

        public LuckyNumberService(DaprClient daprClient)
        {
            _daprClient = daprClient;
        }

        public async Task<int> GetWinnerNumber(int roundId, int minNumber, int maxNumber)
        {
            //service to service invocation
            var winnerNumber = await _daprClient.InvokeMethodAsync<int>(
                HttpMethod.Get,
                "luckynumberservice",
                "/api/LuckyNumber/GetWinnerNumber/"+roundId + "/" + minNumber + "/" + maxNumber
                );

            return winnerNumber;
        }

        public async Task PublishWinner(string winnerName)
        {
            //pubsub
            await _daprClient.PublishEventAsync("my-pubsub", "newWinner", winnerName);
        }
    }
}

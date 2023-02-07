namespace SpyrosoftLearn.Services
{
    public interface ILuckyNumberService
    {
        Task<int> GetWinnerNumber(int roundId, int minNumber, int maxNumber);

        Task PublishWinner(string winnerName);
    }
}

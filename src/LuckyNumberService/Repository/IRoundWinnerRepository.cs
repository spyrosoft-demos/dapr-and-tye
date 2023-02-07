namespace LuckyNumberService.Repository
{
    public interface IRoundWinnerRepository
    {
        int GetWinnerNumber(int roundId, int minNumber, int maxNumber);
    }
}

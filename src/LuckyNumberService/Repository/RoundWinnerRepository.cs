using LuckyNumberService.DbContexts;
using LuckyNumberService.Models;

namespace LuckyNumberService.Repository
{
    public class RoundWinnerRepository : IRoundWinnerRepository
    {
        private readonly RoundWinnerContext _dbContext;
        private readonly ILogger<RoundWinnerRepository> _logger;

        public RoundWinnerRepository(RoundWinnerContext dbContext, ILogger<RoundWinnerRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public int GetWinnerNumber(int roundId, int minNumber, int maxNumber)
        {
            Random rnd = new();
            int winnerNumber = rnd.Next(minNumber, (maxNumber + 1));

            _logger.LogInformation("round winning number: {RoundWinningNumber}", winnerNumber);

            try
            {
                RoundWinner roundWinner = new()
                {
                    RoundId = roundId,
                    MinNumber = minNumber,
                    MaxNumber = maxNumber,
                    CreatedOn = DateTime.Now,
                    WinnerNumber = winnerNumber
                };

                _dbContext.RoundWinners.Add(roundWinner);
                _dbContext.SaveChanges();

                _logger.LogInformation("round {RoundId} winner inserted, ID: {RoundWinnerId}", roundId, roundWinner.Id);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "error saving roundWinner");
                return 0;
            }

            return winnerNumber;
        }

    }
}

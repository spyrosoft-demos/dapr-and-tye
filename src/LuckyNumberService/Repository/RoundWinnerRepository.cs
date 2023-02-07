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
            Random rnd = new Random();
            int winnerNumber = rnd.Next(minNumber, (maxNumber + 1));

            _logger.LogInformation($"Winner number is {winnerNumber}");

            try
            {
                RoundWinner roundWinner = new RoundWinner()
                {
                    RoundId = roundId,
                    MinNumber = minNumber,
                    MaxNumber = maxNumber,
                    CreatedOn = DateTime.Now,
                    WinnerNumber = winnerNumber
                };

                _dbContext.RoundWinners.Add(roundWinner);
                _dbContext.SaveChanges();

                _logger.LogInformation($"Winner number {winnerNumber} is saved with id {roundWinner.Id}");
            }
            catch(Exception ex)
            {
                _logger.LogError("Error saving roundWinner, ex : "+ ex);
                return 0;
            }

            return winnerNumber;
        }

    }
}

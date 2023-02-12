using Microsoft.AspNetCore.Identity;
using SpyrosoftLearn.Controllers;
using SpyrosoftLearn.Data;
using SpyrosoftLearn.Dtos;
using SpyrosoftLearn.Services.Interfaces;

namespace SpyrosoftLearn.Services
{
    public class GameResultService : IGameResultService
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _context;

        public GameResultService(ApplicationDbContext context, ILogger<HomeController> logger, UserManager<IdentityUser> userManager) 
        {
            _context = context;
            _logger = logger;
            _userManager = userManager;
        }

        public List<GameResultDto> GetGameResults()
        {
            var gameResultDtos = new List<GameResultDto>();

            try
            {
                gameResultDtos = _context.CatchTheTimes
                .GroupBy(c => c.UserId)
                .Select(x => new GameResultDto
                {
                    UserId = x.First().UserId,
                    UserName = x.First().UserName,
                    NumberOfPoints = x.Sum(p => p.NumberOfPoints),
                    NumberOfLuckyPoints = 0,
                    TotalPoints = x.Sum(p => p.NumberOfPoints)
                })
                .OrderByDescending(x => x.NumberOfPoints)
                .ToList();

                if (!gameResultDtos.Any())
                {
                    return gameResultDtos;
                }

                var roundWinnerUserIds = _context.Rounds.Where(x => x.WinnerUserId != null).Select(x => x.WinnerUserId).ToList();

                foreach (var winnerUserId in roundWinnerUserIds)
                {
                    var user = gameResultDtos.SingleOrDefault(x => x.UserId == winnerUserId);
                    if (user != null)
                    {
                        user.NumberOfLuckyPoints = 50;
                        user.TotalPoints += 50;
                    }
                }                
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "GetGameResults exception");
            }

            return gameResultDtos;
        }
    }
}

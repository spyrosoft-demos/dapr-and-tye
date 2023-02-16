using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SpyrosoftLearn.Controllers;
using SpyrosoftLearn.Data;
using SpyrosoftLearn.Dtos;
using SpyrosoftLearn.Services.Interfaces;

namespace SpyrosoftLearn.Services;

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

    public async Task<List<GameResultDto>> GetGameResults()
    {
        var gameResultDtos = new List<GameResultDto>();

        try
        {
            var activeRound = await _context.Rounds.FirstOrDefaultAsync(r => r.IsActive == true);

            if (activeRound == null)
            {
                return gameResultDtos;
            }

            gameResultDtos = _context.CatchTheTimes
            .Where(x => x.RoundId == activeRound.Id)
            .GroupBy(c => c.UserId)            
            .Select(x => new GameResultDto
            {
                UserId = x.First().UserId,
                UserName = x.First().UserName,
                NumberOfPoints = x.Sum(p => p.NumberOfPoints),
                NumberOfLuckyPoints = 0,
                TotalPoints = x.Sum(p => p.NumberOfPoints)                
            })            
            .ToList();

            if (!gameResultDtos.Any())
            {
                return gameResultDtos;
            }

            var roundWinnerUser = await _context.Rounds
                .SingleOrDefaultAsync(x => x.WinnerUserId != null && x.Id == activeRound.Id);

            if(roundWinnerUser != null)
            {
                var user = gameResultDtos.SingleOrDefault(x => x.UserId == roundWinnerUser.WinnerUserId);
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

        return gameResultDtos.OrderByDescending(x => x.TotalPoints).ToList();
    }
}

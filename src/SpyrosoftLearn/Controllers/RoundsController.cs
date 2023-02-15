using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpyrosoftLearn.Data;
using SpyrosoftLearn.Models;
using SpyrosoftLearn.Services.Interfaces;
using System.Data;

namespace SpyrosoftLearn.Controllers
{    
    public class RoundsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILuckyNumberService _luckyNumberService;
        private readonly ILogger<RoundsController> _logger;

        public RoundsController(ApplicationDbContext context, UserManager<IdentityUser> userManager, ILogger<RoundsController> logger, ILuckyNumberService luckyNumberService)
        {
            _context = context;
            _userManager = userManager;
            _luckyNumberService = luckyNumberService;
            _logger = logger;
        }

        // GET: Rounds
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
              return _context.Rounds != null ? 
                          View(await _context.Rounds.OrderByDescending(r => r.CreatedOn).ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.Rounds'  is null.");
        }

        // GET: Rounds/Details/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null || _context.Rounds == null)
            {
                return NotFound();
            }

            var round = await _context.Rounds
                .FirstOrDefaultAsync(m => m.Id == id);
            if (round == null)
            {
                return NotFound();
            }

            return View(round);
        }

        // GET: Rounds/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {

            var round = new Round
            {
                CreatorName = User.Identity.Name,
                IsActive = true,
                CreatedOn = DateTime.UtcNow
            };

            return View(round);
        }

        // POST: Rounds/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Id,Name,CreatorName,IsActive,CreatedOn,FinishedOn")] Round round)
        {
            if (ModelState.IsValid)
            {
                _context.Add(round);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Created new round, id: {round.Id}, name: {round.Name}");

                var activeRounds = _context.Rounds.Where(r => r.Id != round.Id);
                foreach(var activeRound in activeRounds)
                {
                    activeRound.IsActive = false;
                    await _context.SaveChangesAsync();
                }

                _logger.LogInformation($"All rounds set inactive except new round, id: {round.Id}, name: {round.Name}");

                return RedirectToAction(nameof(Index));
            }
            return View(round);
        }

        // GET: Rounds/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Rounds == null)
            {
                return NotFound();
            }

            var round = await _context.Rounds.FindAsync(id);
            if (round == null)
            {
                return NotFound();
            }
            return View(round);
        }

        // POST: Rounds/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(long id, [Bind("Id,Name,CreatorName,IsActive,CreatedOn,FinishedOn")] Round round)
        {
            if (id != round.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if(round.IsActive == false)
                    {
                        round.FinishedOn = DateTime.UtcNow;
                    }
                    else
                    {
                        round.FinishedOn = null;
                    }

                    _context.Update(round);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RoundExists(round.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(round);
        }

        // GET: Rounds/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null || _context.Rounds == null)
            {
                return NotFound();
            }

            var round = await _context.Rounds
                .FirstOrDefaultAsync(m => m.Id == id);
            if (round == null)
            {
                return NotFound();
            }

            return View(round);
        }

        // POST: Rounds/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Rounds == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Rounds'  is null.");
            }
            var round = await _context.Rounds.FindAsync(id);
            if (round != null)
            {
                _context.Rounds.Remove(round);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RoundExists(int id)
        {
          return (_context.Rounds?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GenerateWinnerNumber(int roundId)
        {
            try
            {
                var minNumber = _context.LuckyNumbers.Where(n => n.RoundId == roundId).Min(n => n.Id);
                var maxNumber = _context.LuckyNumbers.Where(n => n.RoundId == roundId).Max(n => n.Id);

                _logger.LogInformation("Calling lucky number service, minNumber {MinNumber}, maxNumber {MaxNumber}", minNumber, maxNumber);

                var winnerNumber = await _luckyNumberService.GetWinnerNumber(roundId, minNumber, maxNumber);

                _logger.LogInformation("Lucky number service generated winner, lucky number is {WinnerNumber}", winnerNumber);

                if (winnerNumber == 0)
                {
                    return RedirectToAction("Error", "Home", new { message = "Winner number not generated." });
                }

                var winner = await _context.LuckyNumbers.FirstAsync(l => l.Id == winnerNumber);
                if (winner == null)
                {
                    return RedirectToAction("Error", "Home", new { message = "Lucky number can't be found, Id: {WinnerNumber}", winnerNumber });
                }

                var round = await _context.Rounds.SingleAsync(r => r.Id == roundId);
                round.MinNumber = minNumber;
                round.MaxNumber = maxNumber;
                round.IsActive = false;
                round.WinnerNumber = winnerNumber;
                round.WinnerUserId = winner.UserId;
                round.FinishedOn = DateTime.Now;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Round with id {RoundId} is set inactive, winner number is {WinnerNumber}", round.Id, winnerNumber);

                

                await _luckyNumberService.PublishWinner(winner.UserName);

                _logger.LogInformation("Published winner name {WinnerUserName}", winner.UserName);

                ViewBag.WinnerName = winner.UserName;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "GenerateWinnerNumber exception");
                return RedirectToAction("Error", "Home", new { message = "Error generating winner number." });
            }         

            return View();
        }

        [Authorize(Roles = "Admin,Default")]
        public async Task<IActionResult> JoinRound(int roundId)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(User.Identity.Name);

                if (user == null)
                {
                    return RedirectToAction("Error", "Home", new { message = "User can't be found, name: " + User.Identity.Name });
                }

                var luckyNumberUser = _context.LuckyNumbers.FirstOrDefault(x => x.RoundId == roundId && x.UserId == user.Id);

                if (luckyNumberUser != null)
                {
                    ViewBag.LuckyNumber = luckyNumberUser.Id;
                    return View();
                }

                var luckyNumber = new LuckyNumber
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    RoundId = roundId
                };

                _logger.LogInformation("Creating new lucky number, user name {UserName}, round id {RoundId}", user.UserName, roundId);

                _context.LuckyNumbers.Add(luckyNumber);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Saved lucky number, lucky number id {LuckyNumberId}", luckyNumber.Id);

                ViewBag.LuckyNumber = luckyNumber.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Join round exception, roundId: {RoundId} ", roundId);
                return RedirectToAction("Error", "Home", new { message = "Error joining round id: " + roundId});
            }

            return View();
        }
    }
}

using Google.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpyrosoftLearn.Data;
using SpyrosoftLearn.Dtos;

namespace SpyrosoftLearn.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ApplicationDbContext _context;

    public HomeController(ApplicationDbContext context, ILogger<HomeController> logger, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _logger = logger;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        if (User.Identity?.IsAuthenticated??false)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name??string.Empty);

            if (user == null)
            {
                return RedirectToAction("Error", new { message = "User can't be found." });
            }

            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");

            _logger.LogInformation("Redirecting to user index page.");

            var userConfigNumber = await _context.UserConfigurations.FirstOrDefaultAsync(u => u.UserId == user.Id);
            var numberOfClicks = _context.CatchTheTimes.Count(x => x.UserId == user.Id);

            var intervalDto = new IntervalDto()
            {
                UserName = User.Identity.Name,
                UserId = user.Id,
                UserConfigNumber = userConfigNumber?.Id,
                NumberOfClicks = numberOfClicks,
                IsAdmin = isAdmin
            };

            return View(intervalDto);
        }
        else
        {
            _logger.LogInformation("User is not authenticated.");
        }

        return RedirectToAction("Start");
    }

    [Authorize(Roles = "Admin")]
    public IActionResult AdminIndex()
    {
        return View();
    }

    public IActionResult Start()
    {
        return View();
    }

    [Authorize(Roles = "Admin")]
    public IActionResult Results()
    {
        return View();
    }

    [Authorize(Roles = "Default")]
    public async Task<IActionResult> RoundGame()
    {      
        var user = await _userManager.FindByNameAsync(User.Identity.Name);

        if (user == null)
        {                
            return RedirectToAction("Error", new { message = "User can't be found."});
        }

        var joinedRounds = await _context.LuckyNumbers
            .Where(l => l.UserId == user.Id)
            .Select(l => l.RoundId)
            .Distinct()
            .ToListAsync();

        var round = await _context.Rounds
            .Where(r => r.IsActive == true && !joinedRounds.Contains(r.Id))
            .OrderByDescending(r => r.CreatedOn).ToListAsync();

        return View(round);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error(string message)
    {
        _logger.LogError(message);

        ViewBag.Message = message;
        return View();
    }

    public IActionResult IntervalGame()
    {
        return View();
    }
}
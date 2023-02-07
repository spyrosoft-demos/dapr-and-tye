using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpyrosoftLearn.Data;

namespace SpyrosoftLearn.Controllers
{
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
            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.FindByNameAsync(User.Identity.Name);

                if (user == null)
                {
                    return RedirectToAction("Error", new { message = "User can't be found." });
                }

                var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
                if (isAdmin)
                {
                    _logger.LogInformation("Redirecting to admin index page.");
                    return RedirectToAction("AdminIndex");
                }
                else
                {
                    _logger.LogInformation("Redirecting to user index page.");
                    return RedirectToAction("UserIndex");
                }
            }
            else
            {
                _logger.LogInformation("User is not authenticated.");
            }

            return View();
        }

        public IActionResult AdminIndex()
        {
            return View();
        }

        public async Task<IActionResult> UserIndex()
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

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(string message)
        {
            _logger.LogError(message);

            ViewBag.Message = message;
            return View();
        }
    }
}
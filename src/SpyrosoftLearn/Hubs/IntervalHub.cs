using Microsoft.AspNetCore.SignalR;
using SpyrosoftLearn.Data;
using SpyrosoftLearn.Models;

namespace SpyrosoftLearn.Hubs;

public class IntervalHub : Hub
{
    private readonly ApplicationDbContext _context;

    public IntervalHub(ApplicationDbContext context) 
    {
        _context = context;
    }

    public async Task SendMessage(int userNumber, string userId, string userName, DateTime clickTime)
    {
        var milliseconds = clickTime.Millisecond;

        var catchTheTime = new CatchTheTime()
        {
            UserId = userId,
            UserName = userName,
            ClickTime = clickTime,
            NumberOfPoints = (milliseconds <= 500) ? (1000 - milliseconds) : milliseconds
        };

        _context.Add(catchTheTime);
        _context.SaveChanges();

        await Clients.All.SendAsync("ReceiveMessage", userNumber, userName, clickTime);
    }
}

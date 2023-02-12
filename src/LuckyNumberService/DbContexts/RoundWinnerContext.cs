using LuckyNumberService.Models;
using Microsoft.EntityFrameworkCore;

namespace LuckyNumberService.DbContexts
{
    public class RoundWinnerContext : DbContext
    {
        public RoundWinnerContext(DbContextOptions<RoundWinnerContext> options) : base(options) { }

        public DbSet<RoundWinner> RoundWinners { get; set; }        
    }
}

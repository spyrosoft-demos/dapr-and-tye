using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SpyrosoftLearn.Models;

namespace SpyrosoftLearn.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<LuckyNumber> LuckyNumbers { get; set; }
        public DbSet<Round> Rounds { get; set; }
        public DbSet<CatchTheTime> CatchTheTimes { get; set; }
        public DbSet<UserConfiguration> UserConfigurations { get; set; }
    }
}
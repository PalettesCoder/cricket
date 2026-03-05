using Microsoft.EntityFrameworkCore;
using HRCricket.Api.Models;

namespace HRCricket.Api.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Blog> Blogs { get; set; } = null!;
        public DbSet<Player> Players { get; set; } = null!;
        public DbSet<Match> Matches { get; set; } = null!;
        public DbSet<Schedule> Schedules { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Map names to match MySQL schema if needed
            modelBuilder.Entity<Blog>().ToTable("blogs");
            modelBuilder.Entity<Player>().ToTable("players");
            modelBuilder.Entity<Match>().ToTable("matches");
            modelBuilder.Entity<Schedule>().ToTable("schedule");
        }
    }
}

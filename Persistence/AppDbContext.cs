using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<CatFact> CatFacts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CatFact>()
                .HasKey(c => c.Id);

            modelBuilder.Entity<CatFact>()
                .HasIndex(c => c.FactHash)
                .IsUnique(); // Ensure unique facts
        }
    }
}

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CineVote.Models;

namespace CineVote.Data
{
    public class AppDbContext : IdentityDbContext<Users>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Competition> Competitions { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<CompetitionMovie> CompetitionMovies { get; set; }
        public DbSet<Vote> Votes { get; set; }
        public DbSet<UserCompetition> UserCompetitions { get; set; }
        public DbSet<CompetitionSubscription> CompetitionSubscriptions { get; set; }
        public DbSet<CompetitionResult> CompetitionResults { get; set; }



        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Composite Key for CompetitionMovie (Many-to-Many)
            builder.Entity<CompetitionMovie>()
                .HasKey(cm => new { cm.CompetitionId, cm.MovieId });

            builder.Entity<CompetitionMovie>()
                .HasOne(cm => cm.Competition)
                .WithMany(c => c.CompetitionMovies)
                .HasForeignKey(cm => cm.CompetitionId);

            builder.Entity<CompetitionMovie>()
                .HasOne(cm => cm.Movie)
                .WithMany(m => m.CompetitionMovies)
                .HasForeignKey(cm => cm.MovieId);

            // Prevent duplicate subscriptions
            builder.Entity<UserCompetition>()
                .HasIndex(uc => new { uc.UserId, uc.CompetitionId })
                .IsUnique();
        }
    }
}

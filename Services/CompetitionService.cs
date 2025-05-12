using CineVote.Data;
using CineVote.Models;
using Microsoft.EntityFrameworkCore;

namespace CineVote.Services
{
    public class CompetitionService
    {
        private readonly AppDbContext _context;

        public CompetitionService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> IsCompetitionEndedAsync(int competitionId)
        {
            var competition = await _context.Competitions.FindAsync(competitionId);
            return competition != null && competition.EndTime < DateTime.Now;
        }

        public async Task CalculateAndStoreResultsAsync(int competitionId)
        {
            var competition = await _context.Competitions
                .FirstOrDefaultAsync(c => c.Id == competitionId);

            // If the competition doesn't exist, return early
            if (competition == null) return;

            // Check if the results have already been calculated
            var existingResults = await _context.CompetitionResults
                .AnyAsync(r => r.CompetitionId == competitionId);

            if (existingResults) return;

            // Get the vote counts for the top 3 movies
            var voteGroups = await _context.Votes
                .Where(v => v.CompetitionId == competitionId)
                .GroupBy(v => v.MovieId)
                .Select(g => new { MovieId = g.Key, VoteCount = g.Count() })
                .OrderByDescending(g => g.VoteCount)
                .Take(3)
                .ToListAsync();

            if (!voteGroups.Any()) return;

            // Create the result entries to be saved
            var resultEntries = voteGroups.Select((r, index) => new CompetitionResult
            {
                CompetitionId = competitionId,
                MovieId = r.MovieId,
                VoteCount = r.VoteCount,
                Rank = index + 1
            });

            // Add the result entries to the database
            _context.CompetitionResults.AddRange(resultEntries);

            // Mark the competition as finished
            competition.Finish = true;

            // Save changes to the database
            await _context.SaveChangesAsync();
        }





    }
}

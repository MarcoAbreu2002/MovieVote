using CineVote.Data;
using Microsoft.EntityFrameworkCore;

namespace CineVote.Services
{
    public class CompetitionResultProcessor : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<CompetitionResultProcessor> _logger;

        public CompetitionResultProcessor(IServiceScopeFactory scopeFactory, ILogger<CompetitionResultProcessor> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var localTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.Local);
                    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    var service = scope.ServiceProvider.GetRequiredService<CompetitionService>();

                    var endedCompetitions = await context.Competitions
                        .Where(c => c.EndTime <= localTime && c.Finish == false)
                        .ToListAsync(stoppingToken);

                    foreach (var competition in endedCompetitions)
                    {
                        await service.CalculateAndStoreResultsAsync(competition.Id);
                    }
                }

                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }
    }
}

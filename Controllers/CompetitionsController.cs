using CineVote.Data;
using CineVote.Models;
using CineVote.Repositories.Abstract;
using CineVote.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class CompetitionsController : Controller
{
    private readonly AppDbContext _context;
    private readonly UserManager<Users> _userManager;
    private readonly ITMDBApiService _tmdbService;

    public CompetitionsController(AppDbContext context, ITMDBApiService tmdbService, UserManager<Users> userManager)
    {
        _context = context;
        _tmdbService = tmdbService;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var competitions = await _context.Competitions.ToListAsync();

        var subscribedCompetitionIds = new List<int>();
        var votedCompetitionIds = new List<int>();

        if (User.Identity.IsAuthenticated)
        {
            var userId = _userManager.GetUserId(User);

            subscribedCompetitionIds = await _context.CompetitionSubscriptions
                .Where(cs => cs.UserId == userId)
                .Select(cs => cs.CompetitionId)
                .ToListAsync();

            votedCompetitionIds = await _context.Votes
                .Where(v => v.UserId == userId)
                .Select(v => v.CompetitionId)
                .ToListAsync();
        }

        ViewBag.SubscribedCompetitionIds = subscribedCompetitionIds;
        ViewBag.VotedCompetitionIds = votedCompetitionIds;

        return View(competitions);
    }

    public async Task<IActionResult> Results(int id)
    {
        var competition = await _context.Competitions.FindAsync(id);
        if (competition == null)
        {
            return NotFound();
        }

        var results = await _context.CompetitionResults
            .Where(r => r.CompetitionId == id)
            .OrderBy(r => r.Rank)
            .ToListAsync();

        // Prepare a list to store movie details
        var resultViewModels = new List<CompetitionResultViewModel>();

        // Fetch movie details for each result
        foreach (var result in results)
        {
            var movie = await _context.Movies.FindAsync(result.MovieId); // Fetch the movie from your database (assumed you have a Movies table)

            if (movie != null)
            {
                var movieInfo = await _tmdbService.GetSingleMovieById(movie.Id); 

                if (movieInfo != null && movieInfo.Any())
                {
                    var movieDetails = movieInfo.First(); // Assuming it returns a list, but we only need the first element

                    var resultViewModel = new CompetitionResultViewModel
                    {
                        Rank = result.Rank,
                        MovieTitle = movieDetails["Title"]?.ToString(),
                        MoviePosterPath = movieDetails["PosterPath"]?.ToString(),
                        VoteCount = result.VoteCount
                    };

                    resultViewModels.Add(resultViewModel);
                }
            }
        }

        var model = new ResultsViewModel
        {
            CompetitionId = competition.Id,
            CompetitionTitle = competition.Title,
            Category = competition.Category,
            EndTime = competition.EndTime,
            StartTime = competition.StartTime,
            Results = resultViewModels
        };

        return View(model);
    }



    public IActionResult ThankYou()
    {
        return View();
    }





    // GET: Competitions/Edit/5
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var competition = await _context.Competitions.FindAsync(id);
        if (competition == null) return NotFound();

        return View(competition);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Subscribe(int competitionId)
    {
        var userId = _userManager.GetUserId(User);

        var alreadySubscribed = await _context.CompetitionSubscriptions
            .AnyAsync(s => s.UserId == userId && s.CompetitionId == competitionId);

        if (!alreadySubscribed)
        {
            var subscription = new CompetitionSubscription
            {
                UserId = userId,
                CompetitionId = competitionId
            };

            _context.CompetitionSubscriptions.Add(subscription);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction("Index");
    }


    // POST: Competitions/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Category,StartTime,EndTime")] Competition competition)
    {
        if (id != competition.Id) return NotFound();

        if (competition.Title != "" && competition.Category != "" )
        {
            try
            {
                _context.Update(competition);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Competitions.Any(e => e.Id == competition.Id))
                    return NotFound();
                else
                    throw;
            }
            return RedirectToAction(nameof(Index));
        }
        return View(competition);
    }

    // GET: Competitions/Delete/5
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var competition = await _context.Competitions
            .FirstOrDefaultAsync(m => m.Id == id);

        if (competition == null) return NotFound();

        return View(competition);
    }

    // POST: Competitions/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var competition = await _context.Competitions.FindAsync(id);
        if (competition != null)
        {
            _context.Competitions.Remove(competition);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }


    // GET: Create Competition
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create()
    {
        var model = new CompetitionCreateViewModel
        {
            SearchResults = await _tmdbService.GetPopularMovies()
        };
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var competition = await _context.Competitions
            .Include(c => c.CompetitionMovies)
            .ThenInclude(cm => cm.Movie)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (competition == null)
        {
            return NotFound();
        }

        // Fetch movie details from TMDb
        var movieDetails = new List<Dictionary<string, object>>();

        foreach (var cm in competition.CompetitionMovies)
        {
            var movieInfo = await _tmdbService.GetSingleMovieById(cm.Movie.Id);
            if (movieInfo != null && movieInfo.Any())
            {
                movieDetails.Add(movieInfo.First()); // each result is a Dictionary<string, object>
            }
        }

        var model = new CompetitionDetailsViewModel
        {
            Id = competition.Id,
            Title = competition.Title,
            Category = competition.Category,
            StartTime = competition.StartTime,
            EndTime = competition.EndTime,
            Movies = movieDetails
        };

        return View(model);
    }



    [HttpGet]
    public async Task<IActionResult> Vote(int id)
    {
        var competition = await _context.Competitions
            .Include(c => c.CompetitionMovies)
            .ThenInclude(cm => cm.Movie)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (competition == null)
        {
            return NotFound();
        }

        // Create a list to store movie details as a list of dictionaries
        var movieDictionaries = new List<Dictionary<string, object>>();

        foreach (var cm in competition.CompetitionMovies)
        {
            // Fetch movie details from TMDb API using the movieId
            var movieInfo = await _tmdbService.GetSingleMovieById(cm.Movie.Id);
            if (movieInfo != null && movieInfo.Any())
            {
                // Add the first result from the movieInfo (which is a dictionary) to our list
                movieDictionaries.Add(movieInfo.First()); // each result is a Dictionary<string, object>
            }
        }

        var model = new VoteViewModel
        {
            CompetitionId = competition.Id,
            Title = competition.Title,
            Category = competition.Category,
            StartTime = competition.StartTime,
            EndTime = competition.EndTime,
            Movies = movieDictionaries // Pass the list of dictionaries to the view
        };

        return View(model);
    }


[HttpPost]
[Authorize]
public async Task<IActionResult> SubmitVote(int competitionId, int selectedMovieId)
{
    var userId = _userManager.GetUserId(User);

    // Check if the user has already voted for this competition
    var existingVote = await _context.Votes
        .FirstOrDefaultAsync(v => v.UserId == userId && v.CompetitionId == competitionId);

    if (existingVote != null)
    {
        // If the user has already voted, update their vote
        existingVote.MovieId = selectedMovieId;
        _context.Votes.Update(existingVote);
    }
    else
    {
        // If it's a new vote, create a new record
        var newVote = new Vote
        {
            UserId = userId,
            CompetitionId = competitionId,
            MovieId = selectedMovieId
        };
        _context.Votes.Add(newVote);
    }

    await _context.SaveChangesAsync();

    // Redirect to a Thank You page after voting
    return RedirectToAction("ThankYou");
}



    // POST: Search for movies
    [HttpPost]
    public async Task<IActionResult> SearchMovies([FromBody] CompetitionCreateViewModel model)
    {
        if (string.IsNullOrWhiteSpace(model.MovieSearchQuery))
        {
            var popularMovies = await _tmdbService.GetPopularMovies();
            // Return popular movies as JSON
            return Json(new { searchResults = popularMovies });
        }

        var searchResults = await _tmdbService.GetMovieByName(model.MovieSearchQuery);
        model.SearchResults = searchResults;

        // Return the search results as JSON for AJAX request
        return Json(new { searchResults = searchResults });
    }




    [HttpPost]
    public async Task<IActionResult> CreateCompetition(CompetitionCreateViewModel model)
    {
        if (model.SelectedMovieIds.Count == 0)
        {
            ModelState.AddModelError("", "All fields are required and at least one movie must be selected.");
            return View("Create", model);
        }

        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var competition = new Competition
            {
                Title = model.Title,
                Category = model.Category,
                StartTime = model.StartTime,
                EndTime = model.EndTime,
                Finish = false,
                CompetitionMovies = new List<CompetitionMovie>()
            };

            var newMovies = new List<Movie>();

            foreach (var movieId in model.SelectedMovieIds)
            {
                var movie = await _context.Movies.FindAsync(movieId);

                if (movie == null)
                {
                    movie = new Movie { Id = movieId };
                    newMovies.Add(movie); // collect to insert with IDENTITY_INSERT
                }

                competition.CompetitionMovies.Add(new CompetitionMovie
                {
                    MovieId = movieId,
                    Competition = competition
                });
            }

            if (newMovies.Any())
            {
                await _context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Movies ON");

                foreach (var movie in newMovies)
                {
                    _context.Movies.Add(movie);
                }

                await _context.SaveChangesAsync();

                await _context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Movies OFF");
            }

            _context.Competitions.Add(competition);
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }

        return RedirectToAction("Index", "Home");
    }
}

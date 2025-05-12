using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace CineVote.Models
{
    public class CompetitionCreateViewModel
    {

        public string? MovieSearchQuery { get; set; }

        public List<Dictionary<string, object>> SearchResults { get; set; } = new List<Dictionary<string, object>>();

        // Raw string from form input
        public string SelectedMovieIdsRaw { get; set; }

        // Parsed list from the raw string
        [NotMapped]  // Optional: only if you're using EF and don't want to store this field in DB
        public List<int> SelectedMovieIds
        {
            get
            {
                if (string.IsNullOrWhiteSpace(SelectedMovieIdsRaw))
                    return new List<int>();

                return SelectedMovieIdsRaw
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(id => int.TryParse(id, out var parsedId) ? parsedId : (int?)null)
                    .Where(id => id.HasValue)
                    .Select(id => id.Value)
                    .ToList();
            }
        }

        public string Title { get; set; }
        public string Category { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}

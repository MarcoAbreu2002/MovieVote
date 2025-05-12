using CineVote.Models;

namespace CineVote.Models
{
    public class ResultsViewModel
    {
        public int CompetitionId { get; set; }
        public string CompetitionTitle { get; set; }

        public string Category { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public List<CompetitionResultViewModel> Results { get; set; }
    }

    public class CompetitionResultViewModel
    {
        public int Rank { get; set; }
        public string MovieTitle { get; set; }
        public string MoviePosterPath { get; set; }
        public int VoteCount { get; set; }
    }



}
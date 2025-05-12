using CineVote.Models;

namespace CineVote.Models
{
    public class VoteViewModel
    {
        public int CompetitionId { get; set; }
        public string Title { get; set; }

        public string Category { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public List<Dictionary<string, object>> Movies { get; set; } = new();
        public int? SelectedMovieId { get; set; }
    }

}

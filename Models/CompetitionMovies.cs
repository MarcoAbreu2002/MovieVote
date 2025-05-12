using System.ComponentModel.DataAnnotations.Schema;

namespace CineVote.Models
{
    public class CompetitionMovie
    {
        public int CompetitionId { get; set; }
        public Competition Competition { get; set; }

        public int MovieId { get; set; }
        public Movie Movie { get; set; }
    }
}

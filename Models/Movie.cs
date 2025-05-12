using System.ComponentModel.DataAnnotations;

namespace CineVote.Models
{
    public class Movie
    {
        [Key]
        public int Id { get; set; } // This is the TMDb ID

        public virtual ICollection<CompetitionMovie> CompetitionMovies { get; set; }
        public virtual ICollection<Vote> Votes { get; set; }
    }
}

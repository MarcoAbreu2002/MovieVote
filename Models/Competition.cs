using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CineVote.Models
{
    public class Competition
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Category { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        public bool Finish { get; set; }

        public virtual ICollection<CompetitionMovie> CompetitionMovies { get; set; }
        public virtual ICollection<UserCompetition> UserCompetitions { get; set; }
        public virtual ICollection<Vote> Votes { get; set; }
    }
}

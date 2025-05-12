using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CineVote.Models
{
    public class CompetitionResult
    {
        [Key]
        public int Id { get; set; }

        public int CompetitionId { get; set; }

        public int MovieId { get; set; }

        public int VoteCount { get; set; }

        public int Rank { get; set; }

        [ForeignKey("CompetitionId")]
        public Competition Competition { get; set; }
    }

}

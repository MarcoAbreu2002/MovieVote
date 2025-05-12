using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CineVote.Models
{
    public class CompetitionSubscription
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public int CompetitionId { get; set; }

        public Users User { get; set; }

        public Competition Competition { get; set; }
    }

}

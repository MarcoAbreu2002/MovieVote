namespace CineVote.Models
{
    public class UserCompetition
    {
        public int Id { get; set; }

        public string UserId { get; set; }
        public Users User { get; set; }

        public int CompetitionId { get; set; }
        public Competition Competition { get; set; }
    }
}

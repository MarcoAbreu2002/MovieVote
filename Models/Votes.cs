﻿using System.ComponentModel.DataAnnotations;

namespace CineVote.Models
{
    public class Vote
    {
        public int Id { get; set; }

        public string UserId { get; set; }
        public Users User { get; set; }

        public int CompetitionId { get; set; }
        public Competition Competition { get; set; }

        public int MovieId { get; set; }
        public Movie Movie { get; set; }

        public DateTime VotedAt { get; set; } = DateTime.UtcNow;
    }

}

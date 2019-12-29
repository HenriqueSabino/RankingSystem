using System;

namespace RankingSystem.model.entities
{
    [Serializable]
    public class PlayerRecords
    {
        public int? PlayerId { get; set; }
        public int? MatchesPlayed { get; set; }
        public int? HighScore { get; set; }
        public float? BestTime { get; set; }
        public DateTime LastUpdated { get; set; }

        public PlayerRecords()
        {
        }

        public PlayerRecords(int? PlayerId, int? MatchesPlayed, int? HighScore, float? BestTime, DateTime LastUpdated)
        {
            this.PlayerId = PlayerId;
            this.MatchesPlayed = MatchesPlayed;
            this.HighScore = HighScore;
            this.BestTime = BestTime;
            this.LastUpdated = LastUpdated;
        }

    }
}
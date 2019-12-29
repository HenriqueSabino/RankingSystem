using System;

namespace RankingSystem.model.entities
{
    public class Match
    {
        private DateTime date;
        public DateTime Date { get => date; }

        private Player player;
        public Player Player { get => player; }

        private float duration;
        public float Duration { get => duration; }

        private int score;
        public int Score { get => score; }

        private int itemsCollected;
        public int ItemsCollected { get => itemsCollected; }


        public Match(Player player, float duration, int score, int itemsCollected)
        {
            date = DateTime.Now;
            this.player = player;
            this.duration = duration;
            this.score = score;
            this.itemsCollected = itemsCollected;
        }

    }
}
using System;

namespace RankingSystem.model.entities
{
    public class Player
    {
        private int? id;
        public int? Id { get => id; set => id = value; }

        private string userName;
        public string UserName { get => userName; set => userName = value; }


        public Player()
        {
        }
        public Player(string userName)
        {
            this.id = null;
            this.userName = userName;
        }
    }
}
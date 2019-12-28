namespace RankingSystem.entities
{
    public class Player
    {
        private int id;
        public int Id { get => id; }

        private string userName;
        public string UserName { get => userName; }

        public Player(int id, string userName)
        {
            this.id = id;
            this.userName = userName;
        }
    }
}
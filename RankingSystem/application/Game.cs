using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using RankingSystem.model.entities;
using RankingSystem.model.Services;
using RankingSystem.db;

namespace RankingSystem.application
{
    class Game
    {
        private static PlayerService playerService;

        static void Main(string[] args)
        {
            playerService = new PlayerService();
            Menu();
        }

        static void Menu()
        {
            Player user = null;

            Console.Clear();

            Console.WriteLine("Menu:");
            Console.WriteLine("1. Register a new player.");
            Console.WriteLine("2. Login");
            Console.WriteLine("3. Exit");

            int answer = int.Parse(Console.ReadLine());

            switch (answer)
            {
                case 1:
                    RegisterPlayer(user);
                    break;
                case 2:
                    Login(user);
                    break;
                case 3:
                    DB.CloseConnection();
                    break;
            }
        }

        static void RegisterPlayer(Player user)
        {
            Console.Clear();

            Console.Write("Player's Username: ");
            string username = Console.ReadLine();

            user = new Player();
            user.UserName = username;

            playerService.SaveOrUpdate(user);
            PlayerMenu(user);
        }

        static void Login(Player user)
        {
            Console.Clear();
            Console.Write("Type your user name: ");
            user = playerService.FindByUserName(Console.ReadLine());

            if (user == null)
            {
                Console.WriteLine("Player could not be found.");
                Menu();
            }
            else
            {
                PlayerMenu(user);
            }
        }

        static void PlayerMenu(Player user)
        {
            Console.Clear();

            Console.WriteLine("Welcome {0}", user.UserName);

            Console.WriteLine("Choose an action:");
            Console.WriteLine("1. Change account name");
            Console.WriteLine("2. Delete account");
            Console.WriteLine("3. Logoff");

            int answer = int.Parse(Console.ReadLine());

            switch (answer)
            {
                case 1:
                    ChangeUserName(user);
                    break;
                case 2:
                    DeleteAccount(user);
                    break;
                case 3:
                    Menu();
                    break;
            }
        }

        static void ChangeUserName(Player user)
        {
            Console.Clear();

            Console.Write("Type your new user name: ");
            user.UserName = Console.ReadLine();

            playerService.SaveOrUpdate(user);
            PlayerMenu(user);
        }

        static void DeleteAccount(Player user)
        {
            playerService.Remove(user);
            user = null;
            Menu();
        }

        static void SaveMatch(Match match)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            string path = "Players' data/" + match.Player.Id + ".bin";

            if (!File.Exists(path))
            {
                using (Stream stream = File.Open(path, FileMode.Create))
                {
                    PlayerRecords records = new PlayerRecords(match.Player.Id, 1, match.Score, match.Duration, DateTime.Now);
                    formatter.Serialize(stream, records);
                }
            }
            else
            {
                PlayerRecords records = null;

                using (Stream stream = File.Open(path, FileMode.Open))
                {
                    records = (PlayerRecords)formatter.Deserialize(stream);
                }

                using (Stream stream = File.Open(path, FileMode.Truncate))
                {
                    records.MatchesPlayed++;
                    records.LastUpdated = DateTime.Now;

                    if (match.Duration < records.BestTime)
                        records.BestTime = match.Duration;
                    if (match.Score > records.HighScore)
                        records.HighScore = match.Score;

                    formatter.Serialize(stream, records);
                }
            }
        }

        static void PlayMatch(Player player, float duration, int score, int itemsCollected)
        {
            Match match = new Match(player, duration, score, itemsCollected);
            SaveMatch(match);
        }

        static void DisplayPlayerRecords(Player player)
        {
            try
            {
                string path = "Players' data/" + player.Id + ".bin";
                BinaryFormatter formatter = new BinaryFormatter();

                using (Stream stream = File.Open(path, FileMode.Open))
                {
                    PlayerRecords records = (PlayerRecords)formatter.Deserialize(stream);
                    Console.Write("Username: {0}, ", player.UserName);
                    Console.Write("matches played: {0}, ", records.MatchesPlayed);
                    Console.Write("best time: {0} sec, ", records.BestTime);
                    Console.Write("high score: {0} ", records.HighScore);
                    Console.WriteLine("(last updated at {0})", records.LastUpdated);
                }

            }
            catch (Exception)
            {
                Console.WriteLine("{0}'s data could not be obtained.");
            }
        }
    }
}
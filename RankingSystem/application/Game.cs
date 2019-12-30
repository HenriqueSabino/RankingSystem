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
        private static PlayerRecordsService playerRecordsService;

        static void Main(string[] args)
        {
            playerService = new PlayerService();
            playerRecordsService = new PlayerRecordsService();
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
            Console.WriteLine("1. Register a match");
            Console.WriteLine("2. Change account name");
            Console.WriteLine("3. Delete account");
            Console.WriteLine("4. Logoff");

            int answer = int.Parse(Console.ReadLine());

            switch (answer)
            {
                case 1:
                    RegisterMatch(user);
                    break;
                case 2:
                    ChangeUserName(user);
                    break;
                case 3:
                    DeleteAccount(user);
                    break;
                case 4:
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
            //removing player and their records from the database

            PlayerRecords records = playerRecordsService.FindById(user.Id.Value);
            playerRecordsService.Remove(records);

            playerService.Remove(user);

            user = null;
            Menu();
        }

        static void SaveMatch(Match match)
        {
            /*
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
            */

            //obtaining the records data from the database if it exists
            PlayerRecords records = playerRecordsService.FindById(match.Player.Id.Value);

            //if there's no data on the database, create a new record
            if (records == null)
            {
                records = new PlayerRecords();
                records.PlayerId = match.Player.Id;
                records.MatchesPlayed = 1;
                records.BestTime = match.Duration;
                records.HighScore = match.Score;

                playerRecordsService.SaveOrUpdate(records);
            }
            else
            {
                records.MatchesPlayed++;

                if (match.Duration < records.BestTime)
                {
                    records.BestTime = match.Duration;
                }
                if (match.Score > records.HighScore)
                {
                    records.HighScore = match.Score;
                }

                playerRecordsService.SaveOrUpdate(records);
            }
            PlayerMenu(match.Player);
        }

        static void RegisterMatch(Player user)
        {
            float duration;
            int score, itemsCollected;

            Console.Clear();
            Console.WriteLine("Register a match:");

            Console.Write("Match duration: ");
            duration = float.Parse(Console.ReadLine());

            Console.Write("Your score: ");
            score = int.Parse(Console.ReadLine());

            Console.Write("Items collected: ");
            itemsCollected = int.Parse(Console.ReadLine());

            Match match = new Match(user, duration, score, itemsCollected);
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
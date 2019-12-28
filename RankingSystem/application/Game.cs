using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using RankingSystem.entities;

namespace RankingSystem.application
{
    class Game
    {
        static void Main(string[] args)
        {
            Player player = new Player(1, "HenriqueSabino");

            PlayMatch(player, 60, 1000, 12);
            PlayMatch(player, 65, 1005, 15);

            DisplayPlayerRecords(player);

            Console.ReadKey();
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
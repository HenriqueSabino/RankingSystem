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

        private static BinaryFormatter formatter = new BinaryFormatter();

        private static bool connected = true;

        static void Main(string[] args)
        {
            playerService = new PlayerService();
            playerRecordsService = new PlayerRecordsService();

            // forcing syncronization, if the connected variable is set to true
            ChangeConnectionState(connected);
            Menu();
        }

        static void Menu()
        {
            Player user = null;

            Console.Clear();

            if (connected)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Connected to internet.\n");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Not connected to the internet.\n");
            }

            Console.ForegroundColor = ConsoleColor.White;

            Console.WriteLine("Menu:\n");

            Console.WriteLine("1. Login");
            if (connected)
            {
                Console.WriteLine("2. Register a new player.");
                Console.WriteLine("3. Disconnect internet");
                Console.WriteLine("4. Exit");

                int answer;

                if (!int.TryParse(Console.ReadLine(), out answer))
                {
                    Menu();
                }
                else
                {
                    switch (answer)
                    {
                        case 1:
                            Login(user);
                            break;
                        case 2:
                            RegisterPlayer(user);
                            break;
                        case 3:
                            ChangeConnectionState(false);
                            Menu();
                            break;
                        case 4:
                            DB.CloseConnection();
                            break;
                        default:
                            Menu();
                            break;
                    }
                }
            }
            else
            {
                Console.WriteLine("2. Connect internet");
                Console.WriteLine("3. Exit");

                int answer;

                if (!int.TryParse(Console.ReadLine(), out answer))
                {
                    Menu();
                }
                else
                {
                    switch (answer)
                    {
                        case 1:
                            Login(user);
                            break;
                        case 2:
                            ChangeConnectionState(true);
                            Menu();
                            break;
                        case 3:
                            DB.CloseConnection();
                            break;
                        default:
                            Menu();
                            break;
                    }
                }
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

            if (connected)
            {
                Console.Write("Type your user name: ");
                user = playerService.FindByUserName(Console.ReadLine());

                if (user == null)
                {
                    Menu();
                }
                else
                {
                    string path = "Players' data/" + user.Id + ".usr";

                    if (!File.Exists(path))
                    {
                        using (Stream stream = File.Open(path, FileMode.Create))
                        {
                            formatter.Serialize(stream, user);
                        }
                    }

                    PlayerMenu(user);
                }
            }
            else
            {
                // Getting all users that were used in the device
                string[] files = Directory.GetFiles("Players' data/", "*.usr", SearchOption.TopDirectoryOnly);
                Player[] savedPlayers = new Player[files.Length];

                // Checking if there are any saved players
                if (files.Length != 0)
                {
                    // populating players array
                    for (int i = 0; i < files.Length; i++)
                    {
                        using (Stream stream = File.Open(files[i], FileMode.Open))
                        {
                            savedPlayers[i] = (Player)formatter.Deserialize(stream);
                        }
                    }

                    Console.WriteLine("Choose an account:\n");

                    // displaying all login options
                    for (int i = 0; i < savedPlayers.Length; i++)
                    {
                        Console.WriteLine("{0}. {1}", i + 1, savedPlayers[i].UserName);
                    }

                    int answer;

                    // setting the player depending on the answer
                    if (int.TryParse(Console.ReadLine(), out answer))
                    {
                        if (answer < 1 || answer > savedPlayers.Length)
                        {
                            Login(user);
                            return;
                        }

                        user = savedPlayers[answer - 1];
                        PlayerMenu(user);
                    }

                }
                else
                {
                    Console.WriteLine("There are no players saved in chache.");
                    Console.WriteLine("Try connecting the internet and then logging in first.");
                    Console.ReadKey();
                    Menu();
                }
            }
        }

        static void PlayerMenu(Player user)
        {
            Console.Clear();

            if (connected)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Connected to internet.\n");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Not connected to the internet.\n");
            }
            Console.ForegroundColor = ConsoleColor.White;

            Console.WriteLine("Welcome {0}\n", user.UserName);

            Console.WriteLine("Choose an action:");
            Console.WriteLine("1. Register a match");

            if (connected)
            {
                Console.WriteLine("2. Disconnect internet");
                Console.WriteLine("3. Change account name");
                Console.WriteLine("4. Delete account");
                Console.WriteLine("5. Logoff");

                int answer;

                if (int.TryParse(Console.ReadLine(), out answer))
                {
                    switch (answer)
                    {
                        case 1:
                            RegisterMatch(user);
                            break;
                        case 2:
                            ChangeConnectionState(false);
                            PlayerMenu(user);
                            break;
                        case 3:
                            ChangeUserName(user);
                            break;
                        case 4:
                            DeleteAccount(user);
                            break;
                        case 5:
                            Menu();
                            break;
                        default:
                            PlayerMenu(user);
                            break;
                    }
                }
            }
            else
            {
                Console.WriteLine("2. Connect internet");
                Console.WriteLine("3. Logoff");

                int answer;

                if (int.TryParse(Console.ReadLine(), out answer))
                {
                    switch (answer)
                    {
                        case 1:
                            RegisterMatch(user);
                            break;
                        case 2:
                            ChangeConnectionState(true);
                            PlayerMenu(user);
                            break;
                        case 3:
                            Menu();
                            break;
                        default:
                            PlayerMenu(user);
                            break;
                    }
                }
                else
                {
                    PlayerMenu(user);
                }
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

        static void ChangeConnectionState(bool connected)
        {
            if (connected)
            {
                //syncronizing Player records

                string[] files = Directory.GetFiles("Players' data/", "*.rcd", SearchOption.TopDirectoryOnly);

                foreach (string file in files)
                {
                    PlayerRecords localRecords = null;

                    using (Stream stream = File.Open(file, FileMode.Open))
                    {
                        localRecords = (PlayerRecords)formatter.Deserialize(stream);
                    }

                    //obtaining the records data from the database if it exists
                    PlayerRecords records = playerRecordsService.FindById(localRecords.PlayerId.Value);

                    //if there's no data on the database, update the db with the local data
                    if (records == null)
                    {
                        // nullifying LastUpdated to be inserted in the database
                        localRecords.LastUpdated = null;

                        playerRecordsService.SaveOrUpdate(localRecords);
                        // Saving first in the db to fill the LastUpdated variable

                        using (Stream stream = File.Open(file, FileMode.Truncate))
                        {
                            formatter.Serialize(stream, localRecords);
                        }
                    }
                    else
                    {
                        // if the database data is more recent, player played in this device locally
                        // then logged in another deviced and played connected
                        // in this case, ignore the local data and overwrite it with the data on the db
                        if (records.LastUpdated > localRecords.LastUpdated)
                        {
                            playerRecordsService.SaveOrUpdate(records);

                            using (Stream stream = File.Open(file, FileMode.Truncate))
                            {
                                formatter.Serialize(stream, records);
                            }
                        }
                        // if the local data is more recent or was updated at the same moment on the database
                        // update upon the local data
                        // data may be lost if the player plays offline in another computer or does not
                        // sync the local data before using another device
                        else
                        {
                            playerRecordsService.SaveOrUpdate(localRecords);

                            using (Stream stream = File.Open(file, FileMode.Truncate))
                            {
                                formatter.Serialize(stream, localRecords);
                            }
                        }
                    }
                }

                Game.connected = connected;
            }
            else
            {
                Game.connected = connected;
            }
        }

        static void SaveMatch(Match match)
        {
            if (connected)
            {
                string localFilePath = "Players' data/" + match.Player.Id + ".rcd";

                //obtaining the records data from the database if it exists
                PlayerRecords records = playerRecordsService.FindById(match.Player.Id.Value);

                //if there's no data on the database and nothing locally either, create a new record
                if (records == null && !File.Exists(localFilePath))
                {
                    records = new PlayerRecords();
                    records.PlayerId = match.Player.Id;
                    records.MatchesPlayed = 1;
                    records.BestTime = match.Duration;
                    records.HighScore = match.Score;

                    playerRecordsService.SaveOrUpdate(records);

                    using (Stream stream = File.Open(localFilePath, FileMode.Create))
                    {
                        formatter.Serialize(stream, records);
                    }
                }
                // if there's data locally but nothing on the db, update both
                else if (records == null && File.Exists(localFilePath))
                {
                    PlayerRecords localData = null;

                    using (Stream stream = File.Open(localFilePath, FileMode.Open))
                    {
                        localData = (PlayerRecords)formatter.Deserialize(stream);
                    }

                    localData.MatchesPlayed++;
                    // nullifying LastUpdated to be inserted in the database
                    localData.LastUpdated = null;

                    localData.BestTime = Math.Min(match.Duration, localData.BestTime.Value);
                    localData.HighScore = Math.Max(match.Score, localData.HighScore.Value);

                    playerRecordsService.SaveOrUpdate(localData);
                    // Saving first in the db to fill the LastUpdated variable

                    using (Stream stream = File.Open(localFilePath, FileMode.Truncate))
                    {
                        formatter.Serialize(stream, localData);
                    }
                }
                // if there's data on the db, but nothing locally, first time playing on the device
                // update the db and save locally, ignoring the local save data on the other devices
                else if (records != null && !File.Exists(localFilePath))
                {
                    records.MatchesPlayed++;

                    records.BestTime = Math.Min(match.Duration, records.BestTime.Value);
                    records.HighScore = Math.Max(match.Score, records.HighScore.Value);

                    playerRecordsService.SaveOrUpdate(records);

                    using (Stream stream = File.Open(localFilePath, FileMode.Create))
                    {
                        formatter.Serialize(stream, records);
                    }
                }
                // if there's data locally and on the db, check the newest and update it
                else if (records != null && File.Exists(localFilePath))
                {
                    PlayerRecords localRecords = null;

                    using (Stream stream = File.Open(localFilePath, FileMode.Open))
                    {
                        localRecords = (PlayerRecords)formatter.Deserialize(stream);
                    }

                    // if the database data is more recent, player played in this device locally
                    // then logged in another deviced and played connected
                    // in this case, ignore the local data
                    if (records.LastUpdated > localRecords.LastUpdated)
                    {
                        records.MatchesPlayed++;

                        records.BestTime = Math.Min(match.Duration, records.BestTime.Value);
                        records.HighScore = Math.Max(match.Score, records.HighScore.Value);

                        playerRecordsService.SaveOrUpdate(records);

                        using (Stream stream = File.Open(localFilePath, FileMode.Truncate))
                        {
                            formatter.Serialize(stream, records);
                        }
                    }
                    // if the local data is more recent or was updated at the same moment on the database
                    // update upon the local data
                    else
                    {
                        PlayerRecords localData = null;

                        using (Stream stream = File.Open(localFilePath, FileMode.Open))
                        {
                            localData = (PlayerRecords)formatter.Deserialize(stream);
                        }

                        localData.MatchesPlayed++;

                        localData.BestTime = Math.Min(match.Duration, localData.BestTime.Value);
                        localData.HighScore = Math.Max(match.Score, localData.HighScore.Value);

                        playerRecordsService.SaveOrUpdate(localData);

                        using (Stream stream = File.Open(localFilePath, FileMode.Truncate))
                        {
                            formatter.Serialize(stream, localData);
                        }
                    }
                }
            }
            else
            {
                string localFilePath = "Players' data/" + match.Player.Id + ".rcd";
                PlayerRecords records = null;

                if (!File.Exists(localFilePath))
                {
                    using (Stream stream = File.Open(localFilePath, FileMode.Create))
                    {
                        records = new PlayerRecords();
                        records.PlayerId = match.Player.Id;
                        records.MatchesPlayed = 1;
                        records.LastUpdated = DateTime.Now;
                        records.BestTime = match.Duration;
                        records.HighScore = match.Score;

                        formatter.Serialize(stream, records);
                    }
                }
                else
                {
                    using (Stream stream = File.Open(localFilePath, FileMode.Open))
                    {
                        records = (PlayerRecords)formatter.Deserialize(stream);
                    }

                    records.MatchesPlayed++;
                    records.LastUpdated = DateTime.Now;
                    records.BestTime = Math.Min(match.Duration, records.BestTime.Value);
                    records.HighScore = Math.Max(match.Score, records.HighScore.Value);

                    using (Stream stream = File.Open(localFilePath, FileMode.Truncate))
                    {
                        formatter.Serialize(stream, records);
                    }
                }
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
                string path = "Players' data/" + player.Id + ".rcd";

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
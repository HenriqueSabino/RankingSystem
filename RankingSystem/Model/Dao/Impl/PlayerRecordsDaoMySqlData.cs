using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using RankingSystem.db;
using RankingSystem.model.entities;

namespace RankingSystem.model.Dao.Impl
{
    public class PlayerRecordsDaoMySqlData : PlayerRecordsDao
    {
        private MySqlConnection connection;

        public PlayerRecordsDaoMySqlData(MySqlConnection connection)
        {
            this.connection = connection;
        }

        public void Insert(PlayerRecords obj)
        {
            MySqlCommand command = null;

            try
            {
                command = new MySqlCommand();
                command.Connection = connection;

                DateTime now = DateTime.Now;

                // inserting the new record
                command.CommandText = "INSERT INTO records (player_id, last_updated, matches_played, " +
                                                "best_time, high_score) " +
                                    "VALUES (@1, @2, @3, @4, @5);";

                command.Parameters.AddWithValue("@1", obj.PlayerId);
                command.Parameters.AddWithValue("@2", now);
                command.Parameters.AddWithValue("@3", obj.MatchesPlayed);
                command.Parameters.AddWithValue("@4", obj.BestTime);
                command.Parameters.AddWithValue("@5", obj.HighScore);

                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    // updating when it was last updated
                    obj.LastUpdated = now;
                }
                else
                {
                    throw new DBException("Unexpected error! No rows affected.");
                }
            }
            catch (MySqlException e)
            {
                throw new DBException(e.Message);
            }
        }

        public void Update(PlayerRecords obj)
        {
            MySqlCommand command = null;

            try
            {
                DateTime now = DateTime.Now;

                command = new MySqlCommand();
                command.Connection = connection;

                // updating the records on the db

                command.CommandText = "UPDATE records " +
                                    "SET last_updated = @2, " +
                                    "matches_played = @3, " +
                                    "best_time = @4, " +
                                    "high_score = @5 " +
                                    "WHERE player_id = @1;";

                command.Parameters.AddWithValue("@1", obj.PlayerId);
                command.Parameters.AddWithValue("@2", now);
                command.Parameters.AddWithValue("@3", obj.MatchesPlayed);
                command.Parameters.AddWithValue("@4", obj.BestTime);
                command.Parameters.AddWithValue("@5", obj.HighScore);

                command.ExecuteNonQuery();

                // updating when it was last updated
                obj.LastUpdated = now;
            }
            catch (MySqlException e)
            {
                throw new DBException(e.Message);
            }
        }

        public void DeleteById(int id)
        {
            MySqlCommand command = null;

            try
            {

                command = new MySqlCommand();
                command.Connection = connection;

                // deleting all records of a certain player
                command.CommandText = "DELETE FROM records " +
                                    "WHERE player_id = @1;";

                command.Parameters.AddWithValue("@1", id);

                command.ExecuteNonQuery();
            }
            catch (MySqlException e)
            {
                throw new DBException(e.Message);
            }
        }

        public PlayerRecords FindById(int id)
        {
            MySqlCommand command = null;
            MySqlDataReader reader = null;

            try
            {
                command = new MySqlCommand();
                command.Connection = connection;

                command.CommandText = "SELECT * FROM records " +
                                    "WHERE player_id = @1;";

                command.Parameters.AddWithValue("@1", id);

                reader = command.ExecuteReader();

                if (reader.Read())
                {
                    PlayerRecords records = InstantiatePlayerRecords(reader);
                    reader.Close();
                    return records;
                }
                else
                {
                    reader.Close();
                    return null;
                }
            }
            catch (MySqlException e)
            {
                throw new DBException(e.Message);
            }
        }

        public List<PlayerRecords> FindAll()
        {
            MySqlCommand command = null;
            MySqlDataReader reader = null;

            try
            {
                string commandStr = string.Format("SELECT * FROM records");

                command = new MySqlCommand(commandStr, connection);
                reader = command.ExecuteReader();

                List<PlayerRecords> allRecords = new List<PlayerRecords>();

                while (reader.Read())
                {
                    allRecords.Add(InstantiatePlayerRecords(reader));
                }

                reader.Close();

                return allRecords;
            }
            catch (MySqlException e)
            {
                throw new DBException(e.Message);
            }
        }

        private PlayerRecords InstantiatePlayerRecords(MySqlDataReader reader)
        {
            PlayerRecords records = new PlayerRecords();

            records.PlayerId = reader.GetInt32("player_id");
            records.LastUpdated = reader.GetDateTime("last_updated");
            records.MatchesPlayed = reader.GetInt32("matches_played");
            records.BestTime = reader.GetFloat("best_time");
            records.HighScore = reader.GetInt32("high_score");

            return records;
        }
    }
}
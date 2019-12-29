using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using RankingSystem.db;
using RankingSystem.model.entities;

namespace RankingSystem.model.Dao.Impl
{
    public class PlayerDaoMySqlData : PlayerDao
    {
        private MySqlConnection connection;

        public PlayerDaoMySqlData(MySqlConnection connection)
        {
            this.connection = connection;
        }

        public void Insert(Player obj)
        {
            MySqlCommand command = null;

            try
            {
                command = new MySqlCommand();
                command.Connection = connection;

                command.CommandText = "INSERT INTO players (user_name) " +
                                    "VALUES (@1); SELECT last_insert_id();";

                command.Parameters.AddWithValue("@1", obj.UserName);

                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    command = new MySqlCommand();
                    command.Connection = connection;

                    command.CommandText = "SELECT last_insert_id() AS id;";

                    MySqlDataReader reader = command.ExecuteReader();

                    if (reader.HasRows)
                    {
                        reader.Read();
                        obj.Id = reader.GetInt32("id");
                    }

                    reader.Close();
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

        public void Update(Player obj)
        {
            MySqlCommand command = null;

            try
            {
                command = new MySqlCommand();
                command.Connection = connection;

                command.CommandText = "UPDATE players " +
                                    "SET user_name = @1 " +
                                    "WHERE id = @2;";

                command.Parameters.AddWithValue("@1", obj.UserName);
                command.Parameters.AddWithValue("@2", obj.Id);

                command.ExecuteNonQuery();
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

                command.CommandText = "DELETE FROM players " +
                                    "WHERE id = @1;";

                command.Parameters.AddWithValue("@1", id);

                command.ExecuteNonQuery();
            }
            catch (MySqlException e)
            {
                throw new DBException(e.Message);
            }
        }

        public Player FindById(int id)
        {
            MySqlCommand command = null;
            MySqlDataReader reader = null;

            try
            {
                command = new MySqlCommand();
                command.Connection = connection;

                command.CommandText = "SELECT * FROM players " +
                                    "WHERE id = @1;";

                command.Parameters.AddWithValue("@1", id);

                reader = command.ExecuteReader();

                if (reader.Read())
                {
                    Player player = InstantiatePlayer(reader);
                    reader.Close();
                    return player;
                }
                else
                    return null;
            }
            catch (MySqlException e)
            {
                throw new DBException(e.Message);
            }
        }

        public Player FindByUserName(string userName)
        {
            MySqlCommand command = null;
            MySqlDataReader reader = null;

            try
            {
                command = new MySqlCommand();
                command.Connection = connection;

                command.CommandText = "SELECT * FROM players " +
                                    "WHERE user_name = @1";

                command.Parameters.AddWithValue("@1", userName);

                reader = command.ExecuteReader();

                if (reader.Read())
                {
                    Player player = InstantiatePlayer(reader);
                    reader.Close();
                    return player;
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

        public List<Player> FindAll()
        {
            MySqlCommand command = null;
            MySqlDataReader reader = null;

            try
            {
                string commandStr = string.Format("SELECT * FROM players");

                command = new MySqlCommand(commandStr, connection);
                reader = command.ExecuteReader();

                List<Player> players = new List<Player>();

                while (reader.Read())
                {
                    players.Add(InstantiatePlayer(reader));
                }
                reader.Close();
                return players;
            }
            catch (MySqlException e)
            {
                throw new DBException(e.Message);
            }
        }

        private Player InstantiatePlayer(MySqlDataReader reader)
        {
            Player player = new Player();

            player.Id = reader.GetInt32("id");
            player.UserName = reader.GetString("user_name");

            return player;
        }
    }
}
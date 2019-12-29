using System.Data;
using MySql.Data.MySqlClient;

namespace RankingSystem.db
{
    public static class DB
    {
        private static MySqlConnection connection;
        private static string server;
        private static string database;
        private static string uid;
        private static string password;

        private static void Initialize()
        {
            server = "localhost";
            database = "game";
            uid = "dev";
            password = "1234567";

            string connectionStr = "SERVER=" + server + ";" + "DATABASE=" +
            database + ";" + "UID=" + uid + ";" + "PASSWORD=" + password + ";";

            connection = new MySqlConnection(connectionStr);
        }

        public static MySqlConnection GetConnection()
        {
            if (connection == null)
            {
                try
                {
                    Initialize();
                    connection.Open();
                }
                catch (MySqlException e)
                {
                    switch (e.Number)
                    {
                        case 0:

                            //Could not connect to the server
                            throw new DBException("Could not connect to the server.");
                        case 1045:

                            //Invalid username/password
                            throw new DBException("Invalid username/password.");
                        default:
                            throw new DBException(e.Message);
                    }
                }
            }

            return connection;
        }

        public static bool CloseConnection()
        {
            try
            {
                connection.Close();
                return true;
            }
            catch (MySqlException e)
            {
                //Wrapping the MySql exception to my own
                throw new DBException(e.Message);
            }
        }
    }
}
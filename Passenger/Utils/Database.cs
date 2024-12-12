using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using PassengerLib;
using System.Security;

namespace Passenger.Utils
{
    public static class Database
    {
        private static readonly string DefaultPath = @".\data.db";
        public static void InitializeTables()
        {
            if (!File.Exists(DefaultPath))
            {
                using var connection = new SQLiteConnection($"Data source={DefaultPath}");
                connection.Open();
                SQLiteCommand command = new SQLiteCommand(connection);
                command.CommandText = "CREATE TABLE IF NOT EXISTS Users(" +
                    "Id INTEGER PRIMARY KEY AUTOINCREMENT, " +
                    "Login TEXT NOT NULL, " +
                    "Password TEXT NOT NULL)";
                command.ExecuteNonQuery();

                command.CommandText = "CREATE TABLE IF NOT EXISTS Accounts(" +
                    "Id INTEGER PRIMARY KEY AUTOINCREMENT, " +
                    "OwnerId Integer NOT NULL, " +
                    "Service TEXT NOT NULL, Login TEXT NOT NULL, " +
                    "Password TEXT NOT NULL, " +
                    "FOREIGN KEY(OwnerId) REFERENCES Users (Id))";
                command.ExecuteNonQuery();
                connection.Close();
            }
        }

        public static void RegisterUser(string login, SecureString password)
        {
            using var connection = new SQLiteConnection($"Data source ={DefaultPath}");
            connection.Open();

            connection.Close();
        }

        public static List<Accounts> AuthorizeUser()
        {
            return new List<Accounts>;
        }
        
        public static void DeleteUser()
        {

        }

        public static void AddAccount() { }

        public static void DeleteAccount() { }

    }
}

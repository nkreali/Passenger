using PassengerLib;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Security;
using System.Windows.Controls;
using System.DirectoryServices.ActiveDirectory;
using System.Xml.Linq;
using System.Security.Policy;

namespace Passenger.Utils
{
    public class Database
    {
        private static readonly string DefaultPath = @".\data.db";

        public static void InitializeTables() // здесь все готово
        {
            try
            {
                if (!File.Exists(DefaultPath))
                {
                    using (var connection = new SQLiteConnection($"Data source={DefaultPath}"))
                    {
                        connection.Open();
                        SQLiteCommand command = new SQLiteCommand(connection);
                        command.CommandText = "CREATE TABLE IF NOT EXISTS Users(" +
                                              "Id INTEGER PRIMARY KEY AUTOINCREMENT, " +
                                              "Name TEXT NOT NULL, " +
                                              "Password TEXT NOT NULL," +
                                              "DateCreated DATETIME NOT NULL)";
                        command.ExecuteNonQuery();
                    }

                    using (var connection = new SQLiteConnection($"Data source={DefaultPath}"))
                    {
                        connection.Open();
                        SQLiteCommand command = new SQLiteCommand(connection);
                        command.CommandText = "CREATE TABLE IF NOT EXISTS Accounts(" +
                                              "Id INTEGER PRIMARY KEY AUTOINCREMENT, " +
                                              "OwnerId Integer NOT NULL, " +
                                              "Service TEXT NOT NULL, Login TEXT NOT NULL, " +
                                              "Password TEXT NOT NULL, " +
                                              "DateCreated DATETIME NOT NULL, " +
                                              "DateModified DATETIME NOT NULL, " +
                                              "FOREIGN KEY(OwnerId) REFERENCES Users (Id))";
                        command.ExecuteNonQuery();
                    }                        
                }
            }
            catch (Exception ex)
            {
                Notification.ShowNotificationInfo("red", $"Error initializing database: {ex.Message}");
            }
        }

        public static List<User> GetUserList()
        {
            var users = new List<User>();

            using var connection = new SQLiteConnection($"Data source ={DefaultPath}");
            connection.Open();

            SQLiteCommand command = new SQLiteCommand(connection);
            command.CommandText = "SELECT * FROM Users";

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                User user = new User();
                user.Id = Convert.ToInt32(reader["Id"]);
                user.Name = reader["Name"].ToString();
                user.Password = reader["Password"].ToString();
                user.DateCreated = Convert.ToDateTime(reader["DateCreated"]);

                users.Add(user);
            }

            connection.Close();
            return users;
        }

        public static bool isUserExists(string name)
        {
            try
            {
                using (var connection = new SQLiteConnection($"Data source ={DefaultPath}"))
                {
                    connection.Open();

                    SQLiteCommand checkCommand = new SQLiteCommand(connection);
                    checkCommand.CommandText = "SELECT COUNT(*) FROM Users WHERE Name = @Name";
                    checkCommand.Parameters.AddWithValue("@Name", name);
                    var userExists = Convert.ToInt32(checkCommand.ExecuteScalar()) > 0;
                    if (userExists)
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Notification.ShowNotificationInfo("red", ex.Message);
            }

            return false;
        }

        public static void RegisterUser(string name, SecureString password)
        {
            var hash = Convert.ToBase64String(Argon2.Argon2HashPassword
                (PasswordValidator.ConvertSecureStringToString(password)));

            try
            {
                using (var connection = new SQLiteConnection($"Data source ={DefaultPath}"))
                {
                    connection.Open();

                    SQLiteCommand command = new SQLiteCommand(connection);
                    command.CommandText = "INSERT INTO Users(Name, Password, DateCreated) " +
                                          "VALUES (@Name, @Password, @DateCreated)";
                    command.Parameters.AddWithValue("@Name", name);
                    command.Parameters.AddWithValue("@Password", hash);
                    command.Parameters.AddWithValue("@DateCreated", DateTime.Now);
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Notification.ShowNotificationInfo("red", ex.Message);
            }
        }

        public static bool AuthorizeUser(string name, SecureString password)
        {
            try
            {
                using (var connection = new SQLiteConnection($"Data source ={DefaultPath}"))
                {
                    connection.Open();
                    SQLiteCommand command = new SQLiteCommand(connection);

                    command.CommandText = "SELECT Password FROM Users WHERE Name = @Name";
                    command.Parameters.AddWithValue("@Name", name);

                    var hash = Convert.ToBase64String(Argon2.Argon2HashPassword
                               (PasswordValidator.ConvertSecureStringToString(password)));
                    
                    using var reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            string DBhash = reader.GetString(0);
                            if (hash == DBhash)
                            {
                                Globals.masterPassword = password;
                                return true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Notification.ShowNotificationInfo("red", ex.Message);
            }
            return false;
        }

        public static int GetUserID(string username)
        {
            try
            {
                using (var connection = new SQLiteConnection($"Data source ={DefaultPath}"))
                {
                    connection.Open();
                    SQLiteCommand command = new SQLiteCommand(connection);
                    command.CommandText = "SELECT Id FROM Users WHERE Name = @Name";
                    command.Parameters.AddWithValue("@Name", username);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return reader.GetInt32(0);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Notification.ShowNotificationInfo("red", ex.Message);
            }

            return -1;
        }

        public static void DeleteUser(int ownerId)
        {
            try
            {
                using (var connection = new SQLiteConnection($"Data source ={DefaultPath}"))
                {
                    connection.Open();

                    SQLiteCommand command = new SQLiteCommand(connection);
                    command.CommandText = "DELETE FROM Accounts WHERE OwnerId = @OwnerId";
                    command.Parameters.AddWithValue("@OwnerId", ownerId);
                    command.ExecuteNonQuery();

                    connection.Close();
                }

                using (var connection = new SQLiteConnection($"Data source ={DefaultPath}"))
                {
                    connection.Open();

                    SQLiteCommand command = new SQLiteCommand(connection);
                    command.CommandText = "DELETE FROM Users WHERE Id = @OwnerId";
                    command.Parameters.AddWithValue("@OwnerId", ownerId);
                    command.ExecuteNonQuery();

                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Notification.ShowNotificationInfo("red", ex.Message);
            }
        }

        public static List<Account> GetAccountsList(int ownerId)
        {
            List<Account> accounts = new List<Account>();
            try
            {
                using (var connection = new SQLiteConnection($"Data source ={DefaultPath}"))
                {
                    connection.Open();

                    SQLiteCommand command = new SQLiteCommand(connection);
                    command.CommandText = "SELECT * FROM Accounts WHERE OwnerId = @OwnerId";
                    command.Parameters.AddWithValue("@OwnerId", ownerId);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            accounts.Add(new Account
                            {
                                Id = reader.GetInt32(0),
                                Owner_Id = reader.GetInt32(1),
                                Service = reader.IsDBNull(2) ? null : reader.GetString(2),
                                Login = reader.IsDBNull(3) ? null : reader.GetString(3),
                                Password = reader.IsDBNull(4) ? null : reader.GetString(4),
                                DateCreated = reader.IsDBNull(5) ? (DateTime?)null : reader.GetDateTime(5),
                                DateModified = reader.IsDBNull(6) ? (DateTime?)null : reader.GetDateTime(6)
                            });
                        }
                    }

                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Notification.ShowNotificationInfo("red", ex.Message);
            }
            return accounts;
        }

        public static void AddAccount(int ownerId, Account account) 
        {
            try
            {
                using (var connection = new SQLiteConnection($"Data source ={DefaultPath}"))
                {
                    connection.Open();

                    SQLiteCommand command = new SQLiteCommand(connection);
                    command.CommandText = "INSERT INTO Accounts(OwnerId, Service, Login, Password, DateCreated, DateModified) " +
                                          "VALUES (@OwnerId, @Service, @Login, @Password, @DateCreated, @DateModified)";
                    command.Parameters.AddWithValue("@OwnerId", ownerId); 
                    command.Parameters.AddWithValue("@Service", account.Service);
                    command.Parameters.AddWithValue("@Login", account.Login);
                    command.Parameters.AddWithValue("@Password", account.Password);
                    command.Parameters.AddWithValue("@DateCreated", account.DateCreated);
                    command.Parameters.AddWithValue("@DateModified", account.DateModified);

                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Notification.ShowNotificationInfo("red", ex.Message);
            }
        }
        public static bool AccountExists(int ownerId, Account account)
        {
            try
            {
                using (var connection = new SQLiteConnection($"Data source ={DefaultPath}"))
                {
                    connection.Open();

                    SQLiteCommand command = new SQLiteCommand(connection);
                    command.CommandText = "SELECT COUNT(*) FROM Accounts " +
                                               "WHERE OwnerId = @OwnerId AND " +
                                               "Service = @Service AND Login = @Login";
                    command.Parameters.AddWithValue("@OwnerId", ownerId);
                    command.Parameters.AddWithValue("@Service", account.Service);
                    command.Parameters.AddWithValue("@Login", account.Login);

                    var exists = Convert.ToInt32(command.ExecuteScalar()) > 0;
                    if (exists)
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Notification.ShowNotificationInfo("red", ex.Message);
            }
            return false;
        }

        public static void DeleteAccount() { }

    }
}

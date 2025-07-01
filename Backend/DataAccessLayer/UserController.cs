using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Data.Sqlite;
using System.Data.SQLite;

using Kanban.Backend.DataAccessLayer;
namespace Kanban.Backend.DataAccessLayer;


public class UserController
{
    private readonly string _connectionString;
    private readonly string TableName = "User";//maybe need to chnge name to Boards

    public UserController()
    {
        string path = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "kanban.db"));
        this._connectionString = $"Data Source={path};";
    }
    public List<UserDAL> AllUsers()
    {
        var usersList = new List<UserDAL>();
        string selectAll = "SELECT Email, Password FROM User"; // Retrieve Email and Password columns

        try
        {
            using (var connection = new SqliteConnection(this._connectionString))
            using (var command = new SqliteCommand(selectAll, connection))
            {
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    // Check if the query returns results and iterate through them
                    while (reader.Read())
                    {
                        usersList.Add(new UserDAL(
                        reader["Email"].ToString(),
                        reader["Password"].ToString())); // Assuming password is hashed
                    }
                }
            }
            Console.WriteLine($"Loaded {usersList.Count} users.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading users: {ex.Message}");
            return null;
        }
        return usersList;  // Return the list of users
    }

    public bool HasRecords()
    {
        bool hasRecords = false;

        using (var connection = new SQLiteConnection(this._connectionString))
        {
            try
            {
                string query = $"SELECT COUNT(*) FROM {TableName}";

                using (var command = new SQLiteCommand(query, connection))
                {
                    connection.Open();
                    long count = (long)command.ExecuteScalar();
                    hasRecords = count > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in HasRecords: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                return false;
            }
            finally
            {
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }

        return hasRecords;
    }

    public bool DeleteAll()
    {
        bool success = false;
        if (!HasRecords())
        {
            return true;  // Nothing to delete, considered a success
        }
        using (var connection = new SQLiteConnection(this._connectionString))
        {
            try
            {


                string deleteAll = $"DELETE FROM {TableName}";

                using (var command = new SQLiteCommand(deleteAll, connection))
                {
                    connection.Open();
                    int res = command.ExecuteNonQuery();
                    success = res > 0;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in DeleteAll: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                return false;
            }
            finally
            {
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }
        return success;
    }

    /*public bool Insert(UserDAL user)
    {
        try
        {
            if (user == null || string.IsNullOrWhiteSpace(user.Email) || string.IsNullOrWhiteSpace(user.Password))
                throw new Exception("Invalid user data");

            using (var connection = new SQLiteConnection(this._connectionString))
            {
                connection.Open();
                string insert = "INSERT INTO Users (Email, Password) VALUES (@Email, @Password)";
                using (var command = new SQLiteCommand(insert, connection))
                {
                    command.Parameters.AddWithValue("@Email", user.Email);
                    command.Parameters.AddWithValue("@Password", user.Password);

                    int res = command.ExecuteNonQuery();
                    return res > 0;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR][Insert User] {ex.Message}");
            return false;
        }*/
        
        public bool Persist(UserDAL user)
        {
            bool success = false;
            var connection = new SQLiteConnection(this._connectionString);
            try
            {
                string insert = "INSERT INTO User (Email, Password) VALUES (@Email, @Password)";

                SQLiteCommand command = new SQLiteCommand(insert, connection);
                command.Parameters.AddWithValue("@Email", user.Email);
                command.Parameters.AddWithValue("@Password", user.Password);

                connection.Open();
                int res = command.ExecuteNonQuery();
                success = res > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR][Persist User] {ex.Message}");
                return false;
            }
            finally
            {
                connection.Close();
            }

            return success;
        }



}

using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Data.Sqlite;
using System.Data.SQLite;

using Kanban.Backend.DataAccessLayer;
namespace Kanban.Backend.DataAccessLayer;

public class BoardsUsersController
{
    private readonly string _connectionString;
    private readonly string TableName = "BoardsUsers";//maybe need to chnge name to Boards

    public BoardsUsersController()
    {
        string path = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "kanban.db"));
        this._connectionString = $"Data Source={path};";
    }
    public List<string> SelectUsersInBoard(int boardID)
    {
        var members = new List<string>();
        string query = "SELECT Email FROM BoardsUsers WHERE BoardID = @BoardID";

        using (var connection = new SQLiteConnection(this._connectionString))
        using (var command = new SQLiteCommand(query, connection))
        {
            command.Parameters.AddWithValue("@BoardID", boardID);
            connection.Open();
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    members.Add(reader.GetString(0));
                }
            }
        }
        return members;
    }




    public bool JoinUser(int boardID, string userEmail, string type)
    {
        bool success = false;
        string tableName = "BoardsUsers";

        using (var connection = new SQLiteConnection(this._connectionString))
        {
            try
            {
                string insert = $"INSERT INTO {tableName} (BoardID, Email, Type) VALUES (@BoardID, @Email, @Type)";

                using (var command = new SQLiteCommand(insert, connection))
                {
                    command.Parameters.AddWithValue("@BoardID", boardID);
                    command.Parameters.AddWithValue("@Email", userEmail);
                    command.Parameters.AddWithValue("@Type", type); // "Owner" או "Member"
                    connection.Open();
                    int res = command.ExecuteNonQuery();
                    success = res > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error joining user to board: {ex.Message}");
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

    public bool LeaveUser(int boardID, string userEmail)
    {
        bool success = false;
        using (var connection = new SQLiteConnection(this._connectionString))
        {
            try
            {
                string delete = $"DELETE FROM {TableName} WHERE BoardID = @BoardID AND Email = @Email";

                using (var command = new SQLiteCommand(delete, connection))
                {
                    command.Parameters.AddWithValue("@BoardID", boardID);
                    command.Parameters.AddWithValue("@Email", userEmail);
                    connection.Open();
                    int res = command.ExecuteNonQuery();
                    success = res > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error leaving board: {ex.Message}");
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
    public bool DeleteBoard(int BoardID)
    {
        using (var connection = new SQLiteConnection(this._connectionString))
        {
            try
            {
                string delete = $"DELETE FROM {TableName} WHERE BoardID = @BoardID";

                using (var command = new SQLiteCommand(delete, connection))
                {
                    command.Parameters.AddWithValue("@BoardID", BoardID);
                    connection.Open();
                    command.ExecuteNonQuery(); // לא צריך לבדוק את התוצאה כאן
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting board memberships: {ex.Message}");
                return true;
            }
        }
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



        public bool TransferOwnership(int boardID, string currentOwnerEmail, string newOwnerEmail)
    {
        using (var connection = new SQLiteConnection(this._connectionString))
        {
            try
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    // עדכון הבעלים הנוכחי ל־Member
                    string updateCurrentOwner = $"UPDATE {TableName} SET Type = 'Member' WHERE BoardID = @BoardID AND Email = @CurrentOwnerEmail";
                    using (var cmd1 = new SQLiteCommand(updateCurrentOwner, connection))
                    {
                        cmd1.Parameters.AddWithValue("@BoardID", boardID);
                        cmd1.Parameters.AddWithValue("@CurrentOwnerEmail", currentOwnerEmail);
                        cmd1.ExecuteNonQuery();
                    }

                    // עדכון הבעלים החדש ל־Owner
                    string updateNewOwner = $"UPDATE {TableName} SET Type = 'Owner' WHERE BoardID = @BoardID AND Email = @NewOwnerEmail";
                    using (var cmd2 = new SQLiteCommand(updateNewOwner, connection))
                    {
                        cmd2.Parameters.AddWithValue("@BoardID", boardID);
                        cmd2.Parameters.AddWithValue("@NewOwnerEmail", newOwnerEmail);
                        cmd2.ExecuteNonQuery();
                    }

                    transaction.Commit();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in TransferOwnership: {ex.Message}");
                return false;
            }
            finally
            {
                if (connection.State == System.Data.ConnectionState.Open)
                    connection.Close();
            }
        }
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
        



        public bool HasUsersForBoard(int boardId)
        {
            using (var connection = new SQLiteConnection(this._connectionString))
            {
                try
                {
                    string query = $"SELECT COUNT(*) FROM BoardsUsers WHERE BoardID = @BoardID";

                    using (var command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@BoardID", boardId);
                        connection.Open();
                        long count = (long)command.ExecuteScalar();
                        return count > 0;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception in HasUsersForBoard: {ex.Message}");
                    return false;
                }
                finally
                {
                    if (connection.State == System.Data.ConnectionState.Open)
                        connection.Close();
                }
            }
        }

}

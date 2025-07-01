using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Data.Sqlite;
using System.Data.SQLite;

using Kanban.Backend.DataAccessLayer;
namespace Kanban.Backend.DataAccessLayer;

public class BoardController
{
    private readonly string _connectionString;
    private readonly string TableName = "Board";//maybe need to chnge name to Boards

    public BoardController()
    {
        string path = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "kanban.db"));
        this._connectionString = $"Data Source={path};";
    }

    public bool Insert(BoardDAL boardDAL)
    {
        bool success = false;

        using (var connection = new SQLiteConnection(_connectionString))
        {
            try
            {
                string insert = $"INSERT INTO {TableName} (BoardName, Owner) VALUES (@Name, @OwnerEmail)";

                using (var command = new SQLiteCommand(insert, connection))
                {
                    command.Parameters.AddWithValue("@Name", boardDAL.Name);
                    command.Parameters.AddWithValue("@OwnerEmail", boardDAL.OwnerEmail);

                    connection.Open();
                    int res = command.ExecuteNonQuery();
                    success = res > 0;
                }
            }
            catch (Exception ex)
            {
                // אפשר לרשום ללוג במקום קונסול
                return false;
            }
            finally
            {
                connection.Close();
            }
        }

        return success;
    }
    public bool Delete(int boardId)
    {
        int res = -1;

        using (var connection = new SQLiteConnection(_connectionString))
        {
            var command = new SQLiteCommand
            {
                Connection = connection,
                CommandText = $"DELETE FROM {TableName} WHERE BoardID = @BoardID"
            };

            command.Parameters.AddWithValue("@BoardID", boardId);

            try
            {
                connection.Open();
                res = command.ExecuteNonQuery();
            }
            finally
            {
                command.Dispose();
                connection.Close();
            }
        }

        return res > 0;
    }



    public List<BoardDAL> SelectBoardsByUser(string email)
    {
        List<BoardDAL> boards = new List<BoardDAL>();

        using (var connection = new SQLiteConnection(this._connectionString))
        {
            try
            {
                string select = $"SELECT BoardID, BoardName, Owner FROM {TableName} WHERE Owner = @Email";
                Console.WriteLine($"Executing query: {select}");

                SQLiteCommand command = new SQLiteCommand(select, connection);
                command.Parameters.AddWithValue("@Email", email);
                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    if (!reader.HasRows)
                    {
                        Console.WriteLine("Query returned no results.");
                        return boards; // מחזיר רשימה ריקה
                    }

                    while (reader.Read())
                    {
                        int boardId = reader.GetInt32(0);
                        string boardName = reader.GetString(1);
                        string ownerEmail = reader.GetString(2);

                        boards.Add(new BoardDAL(boardId, boardName, ownerEmail,""));
                    }

                    Console.WriteLine($"Loaded {boards.Count} boards.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading boards: {ex.Message}");
                return null;
            }
            finally
            {
                connection.Close();
            }
        }

        return boards;
    }

    public bool Presist(BoardDAL board)
{
    bool success = false;

    using (var connection = new SQLiteConnection(this._connectionString))
    {
        try
        {
            string insert = "INSERT INTO Board (BoardID, BoardName, owner) VALUES (@ID, @Name, @OwnerEmail)";

            using (var command = new SQLiteCommand(insert, connection))
            {
                command.Parameters.AddWithValue("@ID", board.ID);
                command.Parameters.AddWithValue("@Name", board.Name);
                command.Parameters.AddWithValue("@OwnerEmail", board.OwnerEmail);

                connection.Open();
                int res = command.ExecuteNonQuery();
                success = res > 0;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR][Insert Board] {ex.Message}");
            return false;
        }
        finally
        {
            connection.Close();
        }
    }

    return success;
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

    public bool UpdateOwner(string newOwnerEmail, int boardId)
    {
        bool success = false;

        using (var connection = new SQLiteConnection(this._connectionString))
        {
            try
            {
                string updateQuery = "UPDATE Board SET Owner = @NewOwner WHERE BoardID = @BoardID";

                using (var command = new SQLiteCommand(updateQuery, connection))
                {
                    command.Parameters.AddWithValue("@NewOwner", newOwnerEmail);
                    command.Parameters.AddWithValue("@BoardID", boardId);

                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    success = rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating board owner: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                success = false;
            }
            finally
            {
                if (connection.State == System.Data.ConnectionState.Open)
                    connection.Close();
            }
        }

        return success;
    }


  }
using log4net;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using Microsoft.Data.Sqlite;


using Kanban.Backend.DataAccessLayer;

namespace Kanban.Backend.DataAccessLayer
{
    public class ColumnController
    {
        private readonly string _connectionString;
        private readonly string TableName = "Columns";

        public ColumnController()
        {
            string path = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "kanban.db"));
            this._connectionString = $"Data Source={path};";
        }
        public bool Insert(ColmunsDAL column)
        {
            using (var connection = new SQLiteConnection(this._connectionString))
            {
                try
                {
                    string insertQuery = $"INSERT INTO {TableName} (BoardID, ColumnName, TaskLimit) " +
                                            $"VALUES (@BoardID, @ColumnName, @TaskLimit)";

                    using (var command = new SQLiteCommand(insertQuery, connection))
                    {
                        command.Parameters.AddWithValue("@BoardID", column.BoardID);
                        command.Parameters.AddWithValue("@ColumnName", column.ColumnID);
                        command.Parameters.AddWithValue("@TaskLimit", -1);

                        connection.Open();
                        int result = command.ExecuteNonQuery();
                        return result > 0;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception in Insert: {ex.Message}");
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
        }
        public List<ColmunsDAL> Select(int boardID)
        {
            List<ColmunsDAL> columns = new List<ColmunsDAL>();

            using (var connection = new SQLiteConnection(this._connectionString))
            {
                try
                {
                    string select = $"SELECT BoardID, ColumnName, TaskLimit FROM Columns WHERE BoardID = @BoardID";

                    using (var command = new SQLiteCommand(select, connection))
                    {
                        command.Parameters.AddWithValue("@BoardID", boardID);

                        connection.Open();
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int retrievedBoardID = reader.GetInt32(0);
                                string columnName = reader.GetString(1);
                                int taskLimit = reader.IsDBNull(2) ? -1 : reader.GetInt32(2);

                                columns.Add(new ColmunsDAL(retrievedBoardID, columnName, taskLimit));
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error loading columns for board {boardID}: {ex.Message}");
                }
                finally
                {
                    if (connection.State == System.Data.ConnectionState.Open)
                        connection.Close();
                }
            }

            return columns;
        }




        public bool DeleteColumnsByBoardID(int boardID)
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
                    string deleteQuery = $"DELETE FROM {TableName} WHERE BoardID = @BoardID";

                    using (var command = new SQLiteCommand(deleteQuery, connection))
                    {
                        command.Parameters.AddWithValue("@BoardID", boardID);
                        connection.Open();
                        int res = command.ExecuteNonQuery();
                        success = res > 0;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception in DeleteColumnsByBoardID: {ex.Message}");
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


        public bool HasColumnsForBoard(int boardID)
        {
            using (var connection = new SQLiteConnection(this._connectionString))
            {
                try
                {
                    string query = $"SELECT COUNT(*) FROM Columns WHERE BoardID = @BoardID";

                    using (var command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@BoardID", boardID);
                        connection.Open();
                        long count = (long)command.ExecuteScalar();
                        return count > 0;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception in HasColumnsForBoard: {ex.Message}");
                    return false;
                }
                finally
                {
                    if (connection.State == System.Data.ConnectionState.Open)
                        connection.Close();
                }
            }
        }
        


                public bool UpdateColumnLimit(int boardID, string columnID, int limitCol)
        {
            using (var connection = new SQLiteConnection(this._connectionString))
            {
                try
                {
                    string query = $"UPDATE Columns SET TaskLimit = @LimitCol WHERE BoardID = @BoardID AND ColumnName = @ColumnName";

                    using (var command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@LimitCol", limitCol);
                        command.Parameters.AddWithValue("@BoardID", boardID);
                        command.Parameters.AddWithValue("@ColumnName", columnID);

                        connection.Open();
                        int rowsAffected = command.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error updating column limit: {ex.Message}");
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

}
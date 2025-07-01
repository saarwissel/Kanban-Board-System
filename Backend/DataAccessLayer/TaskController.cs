using Kanban.Backend.BusinessLayer;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.SQLite;
using System.IO;
using System.Threading.Tasks;
using System.Transactions;

namespace Kanban.Backend.DataAccessLayer
{
    public class TaskController
    {
        private readonly string _connectionString;
        private readonly string TableName = "Task";

        public TaskController()
        {
            string path = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "kanban.db"));
            this._connectionString = $"Data Source={path};";
        }
        public bool Insert(TaskDAL task)
        {
            using (var connection = new SQLiteConnection(this._connectionString))
            {
                try
                {
                    string insertQuery = $"INSERT INTO {TableName} (BoardID, TaskID, CreationTime, DueDate, Title, Description, Email, TaskStatus) " +
                     $"VALUES (@BoardID, @TaskID, @CreationTime, @DueDate, @Title, @Description, @Email, @TaskStatus)";

                    using (var command = new SQLiteCommand(insertQuery, connection))
                    {
                        command.Parameters.AddWithValue("@BoardID", task.BoardID);
                        command.Parameters.AddWithValue("@TaskID", task.Id);
                        command.Parameters.AddWithValue("@CreationTime", task.CreationTime);
                        command.Parameters.AddWithValue("@DueDate", task.DueDate);
                        command.Parameters.AddWithValue("@Title", task.Title);
                        command.Parameters.AddWithValue("@Description", (object)task.Description ?? DBNull.Value);
                        command.Parameters.AddWithValue("@Email", (object)task.Assignee ?? DBNull.Value);
                        command.Parameters.AddWithValue("@TaskStatus", task.TaskStatus);


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
        public List<TaskDAL> SelectTasksByBoard(int boardID)
        {
            var tasks = new List<TaskDAL>();
            string query = "SELECT TaskID, CreationTime, DueDate, Title, Description, Email, TaskStatus FROM Task WHERE BoardID = @BoardID";

            using (var connection = new SQLiteConnection(this._connectionString))
            {
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@BoardID", boardID);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            tasks.Add(new TaskDAL
                            {
                                Id = reader.GetInt32(0),
                                CreationTime = reader.GetString(1),
                                DueDate = reader.GetString(2),
                                Title = reader.GetString(3),
                                Description = reader.GetString(4),
                                Assignee = reader.IsDBNull(5) ? null : reader.GetString(5),
                                TaskStatus = reader.GetString(6),
                                BoardID = boardID
                            });
                        }
                    }
                }
            }

            return tasks;
        }

        public bool deleteTasksFromBoard(int boardId)
        {
            bool success = false;

            using (var connection = new SQLiteConnection(this._connectionString))
            {
                try
                {
                    string deleteQuery = $"DELETE FROM {TableName} WHERE BoardID = @BoardID";

                    using (var command = new SQLiteCommand(deleteQuery, connection))
                    {
                        command.Parameters.AddWithValue("@BoardID", boardId);
                        connection.Open();
                        int result = command.ExecuteNonQuery();
                        success = result > 0;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception in deleteTasksFromBoard: {ex.Message}");
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

        public bool UpdateAssignee(object newAssignee, int id, int boardId)
        {
            bool success = false;

            using (var connection = new SQLiteConnection(this._connectionString))
            {
                try
                {
                    string update = $"UPDATE Task SET Email = @Email WHERE TaskID = @TaskID AND BoardID = @BoardID";

                    using (var command = new SQLiteCommand(update, connection))
                    {
                        command.Parameters.AddWithValue("@Email", newAssignee ?? DBNull.Value);
                        command.Parameters.AddWithValue("@TaskID", id);
                        command.Parameters.AddWithValue("@BoardID", boardId);

                        connection.Open();
                        int res = command.ExecuteNonQuery();
                        success = res > 0;
                    }
                }
                catch (Exception)
                {
                    return false;
                }
                finally
                {
                    connection.Close();
                }
            }

            return success;
        }

        public bool UpdateTaskStatus(string newStatus, int id, int boardId)
        {
            bool success = false;

            using (var connection = new SQLiteConnection(this._connectionString))
            {
                try
                {
                    string update = $"UPDATE Task SET TaskStatus = @TaskStatus WHERE TaskID = @TaskID AND BoardID = @BoardID";

                    using (var command = new SQLiteCommand(update, connection))
                    {
                        command.Parameters.AddWithValue("@TaskStatus", newStatus);
                        command.Parameters.AddWithValue("@TaskID", id);
                        command.Parameters.AddWithValue("@BoardID", boardId);

                        connection.Open();
                        int res = command.ExecuteNonQuery();
                        success = res > 0;
                    }
                }
                catch (Exception)
                {
                    return false;
                }
                finally
                {
                    connection.Close();
                }
            }

            return success;
        }

        public bool UpdateDueDate(DateTime dueDate, int taskId, int boardId)
        {
            bool success = false;

            using (var connection = new SQLiteConnection(this._connectionString))
            {
                try
                {
                    string updateQuery = "UPDATE Task SET DueDate = @DueDate WHERE TaskID = @TaskID AND BoardID = @BoardID";

                    using (var command = new SQLiteCommand(updateQuery, connection))
                    {
                        command.Parameters.AddWithValue("@DueDate", dueDate.ToString("yyyy-MM-dd HH:mm:ss"));
                        command.Parameters.AddWithValue("@TaskID", taskId);
                        command.Parameters.AddWithValue("@BoardID", boardId);

                        connection.Open();
                        int rowsAffected = command.ExecuteNonQuery();
                        success = rowsAffected > 0;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception in UpdateDueDate: {ex.Message}");
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


        public bool UpdateTitle(string newTitle, int taskId, int boardId)
        {
            bool success = false;

            using (var connection = new SQLiteConnection(this._connectionString))
            {
                try
                {
                    string updateQuery = "UPDATE Task SET Title = @Title WHERE TaskID = @TaskID AND BoardID = @BoardID";

                    using (var command = new SQLiteCommand(updateQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Title", newTitle);
                        command.Parameters.AddWithValue("@TaskID", taskId);
                        command.Parameters.AddWithValue("@BoardID", boardId);

                        connection.Open();
                        int rowsAffected = command.ExecuteNonQuery();
                        success = rowsAffected > 0;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception in UpdateTitle: {ex.Message}");
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

        public bool UpdateDescription(string description, int taskId, int boardId)
        {
            bool success = false;

            using (var connection = new SQLiteConnection(this._connectionString))
            {
                try
                {
                    string updateQuery = "UPDATE Task SET Description = @Description WHERE TaskID = @TaskID AND BoardID = @BoardID";

                    using (var command = new SQLiteCommand(updateQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Description", description ?? string.Empty); // null → ""
                        command.Parameters.AddWithValue("@TaskID", taskId);
                        command.Parameters.AddWithValue("@BoardID", boardId);

                        connection.Open();
                        int rowsAffected = command.ExecuteNonQuery();
                        success = rowsAffected > 0;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception in UpdateDescription: {ex.Message}");
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


        public bool HasTasksForBoard(int boardId)
        {
            using (var connection = new SQLiteConnection(this._connectionString))
            {
                try
                {
                    string query = $"SELECT COUNT(*) FROM {TableName} WHERE BoardID = @BoardID";

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
                    Console.WriteLine($"Exception in HasTasksForBoard: {ex.Message}");
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
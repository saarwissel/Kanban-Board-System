using System;
using System.Data.SQLite;
using System.IO;

namespace BackendUnitTests
{
    public static class TestDatabaseInitializer
    {
        public static void Initialize()
        {
            string path = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "kanban.db"));
            string connectionString = $"Data Source={path};";

            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    // Create User table
                    command.CommandText = @"
                        CREATE TABLE IF NOT EXISTS User (
                            Email TEXT PRIMARY KEY,
                            Password TEXT NOT NULL
                        );";
                    command.ExecuteNonQuery();

                    // Create Board table
                    command.CommandText = @"
                        CREATE TABLE IF NOT EXISTS Board (
                            BoardID INTEGER PRIMARY KEY,
                            BoardName TEXT NOT NULL,
                            Owner TEXT NOT NULL,
                            FOREIGN KEY (Owner) REFERENCES User(Email)
                        );";
                    command.ExecuteNonQuery();

                    // Create Task table
                    command.CommandText = @"
                        CREATE TABLE IF NOT EXISTS Task (
                            TaskID INTEGER,
                            BoardID INTEGER,
                            Email TEXT,
                            Title TEXT NOT NULL CHECK(length(Title) <= 50),
                            Description TEXT CHECK(length(Description) <= 300),
                            DueDate TEXT NOT NULL,
                            CreationTime TEXT NOT NULL,
                            TaskStatus TEXT NOT NULL CHECK (TaskStatus IN ('backlog', 'in progress', 'done')),
                            PRIMARY KEY (TaskID, BoardID),
                            FOREIGN KEY (BoardID) REFERENCES Board(BoardID),
                            FOREIGN KEY (Email) REFERENCES User(Email)
                        );";
                    command.ExecuteNonQuery();

                    // Create Columns table
                    command.CommandText = @"
                        CREATE TABLE IF NOT EXISTS Columns (
                            BoardID INTEGER NOT NULL,
                            ColumnName TEXT NOT NULL CHECK (ColumnName IN ('backlog', 'in progress', 'done')),
                            TaskLimit INTEGER,
                            PRIMARY KEY (BoardID, ColumnName),
                            FOREIGN KEY (BoardID) REFERENCES Board(BoardID)
                        );";
                    command.ExecuteNonQuery();

                    // Create BoardsUsers table
                    command.CommandText = @"
                        CREATE TABLE IF NOT EXISTS BoardsUsers (
                            BoardID INTEGER,
                            Email TEXT,
                            Type TEXT,
                            PRIMARY KEY (BoardID, Email),
                            FOREIGN KEY (BoardID) REFERENCES Board(BoardID),
                            FOREIGN KEY (Email) REFERENCES User(Email)
                        );";
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}

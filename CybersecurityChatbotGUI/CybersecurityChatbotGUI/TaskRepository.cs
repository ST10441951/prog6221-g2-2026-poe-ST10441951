using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace CybersecurityChatbotGUI
{
    /* Part 3 / Task 1: Database Integration (Task Storage).
    This is the data-access layer for the task assistant. It is the ONLY class that talks SQL,
    which keeps the database details out of the chatbot logic and the GUI (separation of
    concerns). Every method:
        - gets its connection from DatabaseConfig (so the connection string lives only in
          App.config),
        - makes sure the table exists first (EnsureInitialized), and
        - uses PARAMETERIZED queries, which is the correct, injection-safe way to build SQL
          (especially fitting for a cybersecurity project).

    References:
    Oracle (2024). Using Connector/NET with Prepared Statements. [Online] MySQL Developer Guide.
    Available at: https://dev.mysql.com/doc/connector-net/en/connector-net-programming-prepared.html
    Microsoft (2023). MySqlCommand.Parameters / SQL injection. [Online] Microsoft Learn.
    Available at: https://learn.microsoft.com/en-us/sql/relational-databases/security/sql-injection
    */
    public class TaskRepository
    {
        /* SQL to create the table the first time the app runs against a fresh database.
        IF NOT EXISTS makes this safe to run every time. */
        private const string CreateTableSql = @"
CREATE TABLE IF NOT EXISTS tasks (
    Id           INT AUTO_INCREMENT PRIMARY KEY,
    Title        VARCHAR(255) NOT NULL,
    Description  TEXT NULL,
    ReminderDate DATETIME NULL,
    IsCompleted  TINYINT(1) NOT NULL DEFAULT 0,
    CreatedAt    DATETIME NOT NULL
);";

        /* Cached so we only run the create-database / create-table step once per session. */
        private bool _initialized = false;

        /* Creates the database (if missing) and the tasks table (if missing). Safe to call
        repeatedly. Throws if MySQL cannot be reached, which callers catch to show a friendly
        "database offline" message instead of crashing. */
        public void Initialize()
        {
            if (!DatabaseConfig.EnsureDatabaseExists(out string message))
                throw new InvalidOperationException(message);

            using MySqlConnection connection = DatabaseConfig.GetConnection();
            connection.Open();
            using var command = new MySqlCommand(CreateTableSql, connection);
            command.ExecuteNonQuery();

            _initialized = true;
        }

        /* Runs Initialize once before any data operation so callers never have to remember to. */
        private void EnsureInitialized()
        {
            if (!_initialized) Initialize();
        }

        /* Inserts a new task and returns its database-generated Id (also set on the object). */
        public int AddTask(TaskItem task)
        {
            EnsureInitialized();

            using MySqlConnection connection = DatabaseConfig.GetConnection();
            connection.Open();

            using var command = new MySqlCommand(
                @"INSERT INTO tasks (Title, Description, ReminderDate, IsCompleted, CreatedAt)
                  VALUES (@title, @description, @reminder, @completed, @created);", connection);

            command.Parameters.AddWithValue("@title", task.Title);
            command.Parameters.AddWithValue("@description", task.Description ?? string.Empty);
            command.Parameters.AddWithValue("@reminder", (object?)task.ReminderDate ?? DBNull.Value);
            command.Parameters.AddWithValue("@completed", task.IsCompleted ? 1 : 0);
            command.Parameters.AddWithValue("@created", task.CreatedAt);

            command.ExecuteNonQuery();

            task.Id = (int)command.LastInsertedId;
            return task.Id;
        }

        /* Returns every task, ordered so pending tasks appear first and newest first within
        each group. Used by both the chat "show tasks" command and the Task Manager window. */
        public List<TaskItem> GetAllTasks()
        {
            EnsureInitialized();

            var tasks = new List<TaskItem>();

            using MySqlConnection connection = DatabaseConfig.GetConnection();
            connection.Open();

            using var command = new MySqlCommand(
                @"SELECT Id, Title, Description, ReminderDate, IsCompleted, CreatedAt
                  FROM tasks
                  ORDER BY IsCompleted ASC, CreatedAt DESC;", connection);

            using MySqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                /* Positional reads (the SELECT column order is fixed above) keep this robust. */
                tasks.Add(new TaskItem
                {
                    Id = reader.GetInt32(0),
                    Title = reader.GetString(1),
                    Description = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                    ReminderDate = reader.IsDBNull(3) ? (DateTime?)null : reader.GetDateTime(3),
                    IsCompleted = reader.GetBoolean(4),
                    CreatedAt = reader.GetDateTime(5)
                });
            }

            return tasks;
        }

        /* Sets (or updates) the reminder date for an existing task. */
        public bool SetReminder(int taskId, DateTime reminderDate)
        {
            EnsureInitialized();

            using MySqlConnection connection = DatabaseConfig.GetConnection();
            connection.Open();

            using var command = new MySqlCommand(
                "UPDATE tasks SET ReminderDate = @reminder WHERE Id = @id;", connection);
            command.Parameters.AddWithValue("@reminder", reminderDate);
            command.Parameters.AddWithValue("@id", taskId);

            return command.ExecuteNonQuery() > 0;
        }

        /* Marks a task as completed in the database. */
        public bool MarkComplete(int taskId)
        {
            EnsureInitialized();

            using MySqlConnection connection = DatabaseConfig.GetConnection();
            connection.Open();

            using var command = new MySqlCommand(
                "UPDATE tasks SET IsCompleted = 1 WHERE Id = @id;", connection);
            command.Parameters.AddWithValue("@id", taskId);

            return command.ExecuteNonQuery() > 0;
        }

        /* Permanently deletes a task from the database. */
        public bool DeleteTask(int taskId)
        {
            EnsureInitialized();

            using MySqlConnection connection = DatabaseConfig.GetConnection();
            connection.Open();

            using var command = new MySqlCommand(
                "DELETE FROM tasks WHERE Id = @id;", connection);
            command.Parameters.AddWithValue("@id", taskId);

            return command.ExecuteNonQuery() > 0;
        }
    }
}

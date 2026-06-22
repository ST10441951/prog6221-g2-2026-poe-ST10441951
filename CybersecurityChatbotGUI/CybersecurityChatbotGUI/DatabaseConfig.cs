using System;
using System.Configuration;
using MySql.Data.MySqlClient;

namespace CybersecurityChatbotGUI
{
    /* Part 3 / Task 1: Central MySQL access point.
    I created this static helper so every database class in the project (such as the
    TaskRepository) gets its connection from ONE place. This keeps the connection string in
    App.config only, demonstrates separation of concerns, and means a marker can repoint the
    whole app at their server by editing App.config alone.

    References:
    Oracle (2024). Connector/NET Programming. [Online] MySQL Developer Guide.
    Available at: https://dev.mysql.com/doc/connector-net/en/connector-net-programming.html
    Microsoft (2023). ConfigurationManager.ConnectionStrings Property. [Online] Microsoft Learn.
    Available at: https://learn.microsoft.com/en-us/dotnet/api/system.configuration.configurationmanager.connectionstrings
    */
    public static class DatabaseConfig
    {
        /* The key that must match the <add name="..."> entry in App.config. */
        private const string ConnectionName = "CyberChatbotDb";

        /* Fallback used only if App.config is missing the entry, so the app still has a
        sensible default for a standard local MySQL install instead of throwing. */
        private const string FallbackConnectionString =
            "server=localhost;port=3306;uid=root;pwd=;database=cyber_chatbot;" +
            "SslMode=None;AllowPublicKeyRetrieval=true;";

        /* Reads the connection string from App.config, falling back to the default above. */
        public static string ConnectionString
        {
            get
            {
                ConnectionStringSettings? setting =
                    ConfigurationManager.ConnectionStrings[ConnectionName];

                return string.IsNullOrWhiteSpace(setting?.ConnectionString)
                    ? FallbackConnectionString
                    : setting!.ConnectionString;
            }
        }

        /* Single factory method for handing out new connections. Callers are responsible for
        opening and disposing (a using-statement) so connections are never leaked. */
        public static MySqlConnection GetConnection()
        {
            return new MySqlConnection(ConnectionString);
        }

        /* Robustness helper (Task 7 edge cases): verifies the server is reachable.
        Returns false with a friendly message instead of throwing so the GUI can show a
        graceful "database offline" notice rather than crashing.
        */
        public static bool TestConnection(out string message)
        {
            try
            {
                using MySqlConnection connection = GetConnection();
                connection.Open();
                message = "Database connection successful.";
                return true;
            }
            catch (Exception ex)
            {
                message = "Could not connect to MySQL: " + ex.Message;
                return false;
            }
        }

        /* Robustness helper: creates the schema database itself if it does not yet exist,
        so the app works on a fresh MySQL install without the marker manually creating it.
        We connect to the server WITHOUT selecting a database, then issue CREATE DATABASE.
        The TaskRepository then creates the tables inside this database.

        Reference:
        Oracle (2024). MySqlConnectionStringBuilder Class. [Online] MySQL Developer Guide.
        Available at: https://dev.mysql.com/doc/dev/connector-net/latest/api/data_api/MySql.Data.MySqlClient.MySqlConnectionStringBuilder.html
        */
        public static bool EnsureDatabaseExists(out string message)
        {
            try
            {
                var builder = new MySqlConnectionStringBuilder(ConnectionString);
                string databaseName = builder.Database;

                /* Connect to the server only (no database selected) so we can create it. */
                builder.Database = string.Empty;

                using var serverConnection = new MySqlConnection(builder.ConnectionString);
                serverConnection.Open();

                /* Backticks guard against reserved words; IF NOT EXISTS makes this idempotent. */
                using var command = new MySqlCommand(
                    $"CREATE DATABASE IF NOT EXISTS `{databaseName}`;", serverConnection);
                command.ExecuteNonQuery();

                message = $"Database '{databaseName}' is ready.";
                return true;
            }
            catch (Exception ex)
            {
                message = "Could not prepare the database: " + ex.Message;
                return false;
            }
        }
    }
}

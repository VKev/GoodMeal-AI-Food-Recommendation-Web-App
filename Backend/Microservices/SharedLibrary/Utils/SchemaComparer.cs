using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Npgsql;

namespace SharedLibrary.Utils
{
    public class SchemaComparer
    {
        private static readonly string SchemaFilePath = "../Infrastructure/Build/track/scaffoldtrack.bin";
        private static readonly string MigrateFilePath = "../Infrastructure/Build/track/migratetrack.bin";

        public static string GenerateDatabaseSchemaHash(string host, int port, string database, string username, string password)
        {
            string connectionString = BuildConnectionString(host, port, database, username, password);
            string schemaDetails = GetDatabaseSchema(connectionString);

            using (var sha256 = SHA256.Create())
            {
                var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(schemaDetails));
                return Convert.ToBase64String(hashBytes);
            }
        }

        private static string BuildConnectionString(string host, int port, string database, string username, string password)
        {
            return $"Host={host};Port={port};Database={database};Username={username};Password={password};";
        }

        private static string GetDatabaseSchema(string connectionString)
        {
            var schemaBuilder = new StringBuilder();

            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                string query = @"
    SELECT 
        c.table_name,
        c.column_name,
        c.data_type,
        c.character_maximum_length,
        tc.constraint_type,
        kcu.constraint_name AS foreign_key_name,
        kcu.table_name AS foreign_table_name,
        kcu.column_name AS foreign_column_name
    FROM 
        information_schema.columns c
    LEFT JOIN 
        information_schema.constraint_column_usage ccu 
        ON c.table_name = ccu.table_name AND c.column_name = ccu.column_name
    LEFT JOIN 
        information_schema.table_constraints tc 
        ON c.table_name = tc.table_name AND tc.constraint_name = ccu.constraint_name
    LEFT JOIN 
        information_schema.key_column_usage kcu 
        ON c.table_name = kcu.table_name AND c.column_name = kcu.column_name
    WHERE 
        c.table_schema = 'public'
    ORDER BY 
        c.table_name, c.ordinal_position;";

                using (var command = new NpgsqlCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string tableName = reader["table_name"].ToString() ?? "None";
                        string columnName = reader["column_name"].ToString() ?? "None";
                        string dataType = reader["data_type"].ToString() ?? "None";
                        string maxLength = reader["character_maximum_length"]?.ToString() ?? "None";
                        string constraintType = reader["constraint_type"]?.ToString() ?? "None";
                        string foreignKeyName = reader["foreign_key_name"]?.ToString() ?? "None";
                        string foreignTableName = reader["foreign_table_name"]?.ToString() ?? "None";
                        string foreignColumnName = reader["foreign_column_name"]?.ToString() ?? "None";

                        schemaBuilder.AppendLine(
                            $"{tableName}|{columnName}|{dataType}({maxLength})|Constraint:{constraintType}|FK:{foreignKeyName}({foreignTableName}.{foreignColumnName})"
                        );
                    }
                }
            }
            return schemaBuilder.ToString();
        }

        public static bool TryGetStoredHash(out string storedHash)
        {
            if (File.Exists(SchemaFilePath))
            {
                byte[] hashBytes = File.ReadAllBytes(SchemaFilePath);
                storedHash = Convert.ToBase64String(hashBytes);
                return true;
            }
            storedHash = "null";
            return false;
        }

        public static void SaveHash(string hash)
        {
            SaveToFile(SchemaFilePath, hash);
            SetMigrationRequired(true);
        }

        public static void SetMigrationRequired(bool required)
        {
            string hash = Convert.ToBase64String(Encoding.UTF8.GetBytes(required.ToString()));
            SaveToFile(MigrateFilePath, hash);
        }

        public static bool IsMigrationRequired()
        {
            if (!File.Exists(MigrateFilePath))
            {
                return false;
            }

            byte[] hashBytes = File.ReadAllBytes(MigrateFilePath);
            string value = Encoding.UTF8.GetString(Convert.FromBase64String(Convert.ToBase64String(hashBytes)));
            return bool.Parse(value);
        }

        private static void SaveToFile(string filePath, string hash)
        {
            string? directoryPath = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath ?? string.Empty);
            }
            byte[] hashBytes = Convert.FromBase64String(hash);
            File.WriteAllBytes(filePath, hashBytes);
        }
    }
} 
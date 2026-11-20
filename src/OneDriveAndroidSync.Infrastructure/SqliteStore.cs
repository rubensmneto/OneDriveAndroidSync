using Microsoft.Data.Sqlite;

namespace OneDriveAndroidSync.Infrastructure;

public sealed class SqliteStore(string databasePath)
{
    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        await using var connection = new SqliteConnection($"Data Source={databasePath}");
        await connection.OpenAsync(cancellationToken);
        await using var command = connection.CreateCommand();
        command.CommandText = "CREATE TABLE IF NOT EXISTS SchemaInfo (Key TEXT PRIMARY KEY, Value TEXT NOT NULL);";
        await command.ExecuteNonQueryAsync(cancellationToken);
    }
}

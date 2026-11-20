using Microsoft.Data.Sqlite;
using OneDriveAndroidSync.Domain;

namespace OneDriveAndroidSync.Infrastructure;

public sealed class FileIndexStore(string databasePath)
{
    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        await using var connection = await OpenConnectionAsync(cancellationToken);
        await using var command = connection.CreateCommand();
        command.CommandText = """
            CREATE TABLE IF NOT EXISTS SourceFiles (
                FullPath TEXT PRIMARY KEY,
                Size INTEGER NOT NULL,
                LastWriteTimeUtc TEXT NOT NULL,
                LastSeenAtUtc TEXT NOT NULL
            );
            """;
        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task UpsertAsync(FileFingerprint fingerprint, CancellationToken cancellationToken = default)
    {
        await using var connection = await OpenConnectionAsync(cancellationToken);
        await using var command = connection.CreateCommand();
        command.CommandText = """
            INSERT INTO SourceFiles (FullPath, Size, LastWriteTimeUtc, LastSeenAtUtc)
            VALUES ($path, $size, $lastWrite, $seen)
            ON CONFLICT(FullPath) DO UPDATE SET
                Size = excluded.Size,
                LastWriteTimeUtc = excluded.LastWriteTimeUtc,
                LastSeenAtUtc = excluded.LastSeenAtUtc;
            """;
        command.Parameters.AddWithValue("$path", fingerprint.FullPath);
        command.Parameters.AddWithValue("$size", fingerprint.Size);
        command.Parameters.AddWithValue("$lastWrite", fingerprint.LastWriteTimeUtc.ToString("O"));
        command.Parameters.AddWithValue("$seen", DateTime.UtcNow.ToString("O"));
        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task<FileFingerprint?> FindAsync(string fullPath, CancellationToken cancellationToken = default)
    {
        await using var connection = await OpenConnectionAsync(cancellationToken);
        await using var command = connection.CreateCommand();
        command.CommandText = "SELECT FullPath, Size, LastWriteTimeUtc FROM SourceFiles WHERE FullPath = $path;";
        command.Parameters.AddWithValue("$path", Path.GetFullPath(fullPath));

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        if (!await reader.ReadAsync(cancellationToken))
        {
            return null;
        }

        return new FileFingerprint(
            reader.GetString(0),
            reader.GetInt64(1),
            DateTime.Parse(reader.GetString(2), null, System.Globalization.DateTimeStyles.RoundtripKind));
    }

    private async Task<SqliteConnection> OpenConnectionAsync(CancellationToken cancellationToken)
    {
        var directory = Path.GetDirectoryName(Path.GetFullPath(databasePath));
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var connection = new SqliteConnection($"Data Source={databasePath};Pooling=False");
        await connection.OpenAsync(cancellationToken);
        return connection;
    }
}

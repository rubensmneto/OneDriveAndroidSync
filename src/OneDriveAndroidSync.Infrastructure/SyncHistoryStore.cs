using Microsoft.Data.Sqlite;
using OneDriveAndroidSync.Application;

namespace OneDriveAndroidSync.Infrastructure;

public sealed record SyncSessionRecord(
    Guid Id,
    DateTime StartedAtUtc,
    DateTime? FinishedAtUtc,
    string SourceRoot,
    string RemoteDestination,
    string? SelectedDeviceId,
    string Status);

public sealed record TransferAttemptRecord(
    Guid Id,
    Guid SessionId,
    string SourcePath,
    string RemotePath,
    string Status,
    long BytesTransferred,
    string? ErrorMessage);

public sealed class SyncHistoryStore(string databasePath)
{
    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        await using var connection = await OpenConnectionAsync(cancellationToken);
        await using var command = connection.CreateCommand();
        command.CommandText = """
            CREATE TABLE IF NOT EXISTS SyncSessions (
                Id TEXT PRIMARY KEY,
                StartedAtUtc TEXT NOT NULL,
                FinishedAtUtc TEXT NULL,
                SourceRoot TEXT NOT NULL,
                RemoteDestination TEXT NOT NULL,
                SelectedDeviceId TEXT NULL,
                Status TEXT NOT NULL
            );
            CREATE TABLE IF NOT EXISTS TransferAttempts (
                Id TEXT PRIMARY KEY,
                SessionId TEXT NOT NULL,
                SourcePath TEXT NOT NULL,
                RemotePath TEXT NOT NULL,
                Status TEXT NOT NULL,
                BytesTransferred INTEGER NOT NULL,
                ErrorMessage TEXT NULL,
                FOREIGN KEY(SessionId) REFERENCES SyncSessions(Id)
            );
            """;
        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task SaveSessionAsync(
        SyncSessionRecord session,
        CancellationToken cancellationToken = default)
    {
        await using var connection = await OpenConnectionAsync(cancellationToken);
        await using var command = connection.CreateCommand();
        command.CommandText = """
            INSERT INTO SyncSessions
                (Id, StartedAtUtc, FinishedAtUtc, SourceRoot, RemoteDestination, SelectedDeviceId, Status)
            VALUES
                ($id, $started, $finished, $source, $destination, $device, $status)
            ON CONFLICT(Id) DO UPDATE SET
                FinishedAtUtc = excluded.FinishedAtUtc,
                SelectedDeviceId = excluded.SelectedDeviceId,
                Status = excluded.Status;
            """;
        command.Parameters.AddWithValue("$id", session.Id.ToString("D"));
        command.Parameters.AddWithValue("$started", session.StartedAtUtc.ToString("O"));
        command.Parameters.AddWithValue("$finished", (object?)session.FinishedAtUtc?.ToString("O") ?? DBNull.Value);
        command.Parameters.AddWithValue("$source", session.SourceRoot);
        command.Parameters.AddWithValue("$destination", session.RemoteDestination);
        command.Parameters.AddWithValue("$device", (object?)session.SelectedDeviceId ?? DBNull.Value);
        command.Parameters.AddWithValue("$status", session.Status);
        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task SaveAttemptAsync(
        TransferAttemptRecord attempt,
        CancellationToken cancellationToken = default)
    {
        await using var connection = await OpenConnectionAsync(cancellationToken);
        await using var command = connection.CreateCommand();
        command.CommandText = """
            INSERT OR REPLACE INTO TransferAttempts
                (Id, SessionId, SourcePath, RemotePath, Status, BytesTransferred, ErrorMessage)
            VALUES
                ($id, $session, $source, $remote, $status, $bytes, $error);
            """;
        command.Parameters.AddWithValue("$id", attempt.Id.ToString("D"));
        command.Parameters.AddWithValue("$session", attempt.SessionId.ToString("D"));
        command.Parameters.AddWithValue("$source", attempt.SourcePath);
        command.Parameters.AddWithValue("$remote", attempt.RemotePath);
        command.Parameters.AddWithValue("$status", attempt.Status);
        command.Parameters.AddWithValue("$bytes", attempt.BytesTransferred);
        command.Parameters.AddWithValue("$error", (object?)attempt.ErrorMessage ?? DBNull.Value);
        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task<SyncSessionRecord?> FindSessionAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        await using var connection = await OpenConnectionAsync(cancellationToken);
        await using var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, StartedAtUtc, FinishedAtUtc, SourceRoot, RemoteDestination, SelectedDeviceId, Status FROM SyncSessions WHERE Id = $id;";
        command.Parameters.AddWithValue("$id", id.ToString("D"));

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        if (!await reader.ReadAsync(cancellationToken))
        {
            return null;
        }

        return new SyncSessionRecord(
            Guid.Parse(reader.GetString(0)),
            DateTime.Parse(reader.GetString(1), null, System.Globalization.DateTimeStyles.RoundtripKind),
            reader.IsDBNull(2) ? null : DateTime.Parse(reader.GetString(2), null, System.Globalization.DateTimeStyles.RoundtripKind),
            reader.GetString(3),
            reader.GetString(4),
            reader.IsDBNull(5) ? null : reader.GetString(5),
            reader.GetString(6));
    }

    private async Task<SqliteConnection> OpenConnectionAsync(CancellationToken cancellationToken)
    {
        var fullPath = Path.GetFullPath(databasePath);
        var directory = Path.GetDirectoryName(fullPath);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var connection = new SqliteConnection($"Data Source={fullPath};Pooling=False");
        await connection.OpenAsync(cancellationToken);
        return connection;
    }
}

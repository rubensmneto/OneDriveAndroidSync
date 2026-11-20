namespace OneDriveAndroidSync.Contracts;

public sealed record SyncSessionSummary(Guid Id, DateTime StartedAt, string Status);

namespace OneDriveAndroidSync.Domain;

public sealed record FileFingerprint(string FullPath, long Size, DateTime LastWriteTimeUtc);

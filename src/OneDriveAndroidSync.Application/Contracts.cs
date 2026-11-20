using OneDriveAndroidSync.Domain;

namespace OneDriveAndroidSync.Application;

public interface IFileSourceScanner { IAsyncEnumerable<FileFingerprint> ScanAsync(string root, CancellationToken cancellationToken = default); }
public interface ISynchronizationPlanner { Task<IReadOnlyList<TransferPlanItem>> CreateAsync(CancellationToken cancellationToken = default); }
public sealed record TransferPlanItem(string SourcePath, string? RemotePath, string Classification);

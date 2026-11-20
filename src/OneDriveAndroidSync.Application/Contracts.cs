using OneDriveAndroidSync.Domain;

namespace OneDriveAndroidSync.Application;

public interface IFileSourceScanner
{
    IAsyncEnumerable<FileFingerprint> ScanAsync(
        string root,
        CancellationToken cancellationToken = default);
}

public interface IFileAnalyzer
{
    Task<IReadOnlyList<FileAnalysisResult>> AnalyzeAsync(
        string root,
        DateRange dateRange,
        CancellationToken cancellationToken = default);
}

public interface IFileFingerprintIndex
{
    Task<FileFingerprint?> FindAsync(
        string fullPath,
        CancellationToken cancellationToken = default);
}

public interface ISynchronizationPlanner
{
    Task<IReadOnlyList<TransferPlanItem>> CreateAsync(
        IReadOnlyList<FileAnalysisResult> files,
        CancellationToken cancellationToken = default);
}

public enum FileClassification
{
    Eligible,
    OutsideDateRange,
    MissingDate,
    InvalidDate
}

public sealed record FileAnalysisResult(
    FileFingerprint Fingerprint,
    FileClassification Classification,
    DateTime? ParsedDate);

public sealed record TransferPlanItem(
    string SourcePath,
    string? RemotePath,
    string Classification);

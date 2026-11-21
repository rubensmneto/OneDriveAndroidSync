namespace OneDriveAndroidSync.Application;

public sealed record TransferRequest(
    string SourcePath,
    string RemotePath,
    long ExpectedSize);

public sealed record TransferProgress(
    string SourcePath,
    string RemotePath,
    long BytesTransferred,
    long TotalBytes);

public sealed record TransferResult(
    string SourcePath,
    string RemotePath,
    long BytesTransferred,
    bool Validated,
    string? Warning = null);

public interface IAdbFileTransferClient
{
    Task PushFileAsync(
        string deviceId,
        string sourcePath,
        string remotePath,
        IProgress<TransferProgress>? progress = null,
        CancellationToken cancellationToken = default);

    Task<long> GetRemoteFileSizeAsync(
        string deviceId,
        string remotePath,
        CancellationToken cancellationToken = default);
}

public interface ITransferService
{
    Task<TransferResult> TransferAsync(
        TransferRequest request,
        IProgress<TransferProgress>? progress = null,
        CancellationToken cancellationToken = default);
}

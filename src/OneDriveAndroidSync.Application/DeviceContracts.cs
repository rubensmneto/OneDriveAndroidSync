namespace OneDriveAndroidSync.Application;

public enum AdbDeviceState
{
    Offline,
    Unauthorized,
    Device
}

public sealed record AdbDeviceInfo(
    string Id,
    AdbDeviceState State,
    string? Model = null);

public sealed record RemoteFileInfo(
    string Path,
    long Size,
    bool IsDirectory);

public interface IAdbClient
{
    Task<IReadOnlyList<AdbDeviceInfo>> ListDevicesAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<RemoteFileInfo>> ListDirectoryAsync(
        string deviceId,
        string remotePath,
        CancellationToken cancellationToken = default);
}

public interface IAndroidDeviceService
{
    Task<IReadOnlyList<AdbDeviceInfo>> RefreshAsync(
        CancellationToken cancellationToken = default);

    Task SelectAsync(
        string deviceId,
        CancellationToken cancellationToken = default);

    AdbDeviceInfo? SelectedDevice { get; }

    Task<IReadOnlyList<RemoteFileInfo>> ListDestinationAsync(
        string remotePath,
        CancellationToken cancellationToken = default);
}

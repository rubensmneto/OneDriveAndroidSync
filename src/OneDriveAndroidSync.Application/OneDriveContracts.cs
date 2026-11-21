namespace OneDriveAndroidSync.Application;

public sealed record OneDriveAvailability(
    string FullPath,
    bool IsAvailable,
    bool IsMaterialized,
    string? Message = null);

public interface IOneDriveAvailabilityService
{
    Task<OneDriveAvailability> CheckAsync(
        string path,
        CancellationToken cancellationToken = default);

    Task<OneDriveAvailability> MaterializeAsync(
        string path,
        CancellationToken cancellationToken = default);
}

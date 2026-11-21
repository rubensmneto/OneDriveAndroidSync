using OneDriveAndroidSync.Application;

namespace OneDriveAndroidSync.Infrastructure;

public sealed class WindowsOneDriveAvailabilityService : IOneDriveAvailabilityService
{
    public Task<OneDriveAvailability> CheckAsync(
        string path,
        CancellationToken cancellationToken = default)
    {
        var fullPath = ValidatePath(path);
        var available = File.Exists(fullPath);
        return Task.FromResult(new OneDriveAvailability(
            fullPath,
            available,
            available,
            available ? null : "O arquivo não está materializado localmente."));
    }

    public async Task<OneDriveAvailability> MaterializeAsync(
        string path,
        CancellationToken cancellationToken = default)
    {
        var fullPath = ValidatePath(path);
        cancellationToken.ThrowIfCancellationRequested();

        if (!File.Exists(fullPath))
        {
            return new OneDriveAvailability(
                fullPath,
                false,
                false,
                "O arquivo somente online não foi materializado automaticamente. " +
                "Abra-o no OneDrive para disponibilizá-lo localmente.");
        }

        await Task.CompletedTask;
        return new OneDriveAvailability(fullPath, true, true);
    }

    private static string ValidatePath(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            throw new ArgumentException("O caminho do arquivo é obrigatório.", nameof(path));
        }

        return Path.GetFullPath(path);
    }
}
